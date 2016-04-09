namespace Game
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(PlayerMover))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class PlayerController : MonoBehaviour
    {
        private const float _velocityIsHitThresholdSqr = 1f;

        [Range(1, 3)]
        public int playerIndex = 1;

        [Range(0.1f, 10f)]
        public float attacksPerSecond = 1f;

        [Range(0.1f, 10f)]
        public float attackRadius = 1f;

        [Range(1f, 1000f)]
        public float hitPower = 10f;

        [Range(0.1f, 10f)]
        public float repairPerSecond = 1f;

        [Range(0.1f, 10f)]
        public float repairRadius = 1f;

        [Range(0.1f, 10f)]
        public float dashPerSecond = 1f;

        public bool isHit
        {
            get;
            private set;
        }

        private PlayerMover _mover;
        private float _lastAttack;
        private float _lastRepair;
        private float _lastDash;
        private float _radius;

        private string horizontalPos = string.Empty;
        private string verticalPos = string.Empty;
        private string horizontalRot = string.Empty;
        private string verticalRot = string.Empty;
        private string interact = string.Empty;

        [SerializeField, ReadOnly]
        private string dash = string.Empty;

        private void OnEnable()
        {
            _mover = this.GetComponent<PlayerMover>();
            if (_mover == null)
            {
                throw new ArgumentNullException("_mover");
            }

            this.horizontalPos = string.Concat("Horizontal_", this.playerIndex);
            this.verticalPos = string.Concat("Vertical_", this.playerIndex);
            this.horizontalRot = string.Concat("RotateHorizontal_", this.playerIndex);
            this.verticalRot = string.Concat("RotateVertical_", this.playerIndex);
            this.interact = string.Concat("Interact_", this.playerIndex);
            this.dash = string.Concat("Dash_", this.playerIndex);

            _radius = this.GetComponent<SphereCollider>().radius;
        }

        private void Update()
        {
            if (PauseManager.isPaused)
            {
                return;
            }

            var horz = Input.GetAxis(this.horizontalPos);
            var vert = Input.GetAxis(this.verticalPos);
            if (horz != 0f || vert != 0f)
            {
                _mover.input = new Vector3(horz, 0f, -vert);
            }

            var hrot = Input.GetAxis(this.horizontalRot);
            var vrot = Input.GetAxis(this.verticalRot);
            if (hrot != 0f || vrot != 0f)
            {
                _mover.Rotate(hrot, vrot);
            }

            var act = Input.GetAxis(this.interact);
            if (act == -1f)
            {
                Attack();
            }

            if (act == 1f)
            {
                Repair();
            }

            if (Input.GetButtonUp(this.dash))
            {
                Dash();
            }

            if (this.isHit)
            {
                if (_mover.velocity.sqrMagnitude < _velocityIsHitThresholdSqr)
                {
                    this.isHit = false;
                }
            }

            // TODO: DEBUG ONLY
            this.transform.rotation = _mover.rotation;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (PauseManager.isPaused)
            {
                return;
            }

            var other = collision.gameObject;
            if (((1 << other.layer) & Layers.instance.wallLayer) == 0)
            {
                // not a wall
                return;
            }

            Debug.Log(this.ToString() + " reflect off wall");
            _mover.Bounce(collision.contacts[0].normal);
        }

        private void Attack()
        {
            var time = Time.timeSinceLevelLoad;
            if (time - _lastAttack < 1f / this.attacksPerSecond)
            {
                return;
            }

            _lastAttack = time;
            Debug.Log(string.Concat("Player ", this.playerIndex, " - Attack"));

            var dir = _mover.rotation * Vector3.forward;
            var hits = Physics.SphereCastAll(this.transform.position + (dir * 0.5f), _radius, dir, this.attackRadius, Layers.instance.playerLayer);
            for (int i = 0; i < hits.Length; i++)
            {
                var hit = hits[i];
                if (ReferenceEquals(hit.transform, this.transform))
                {
                    // Ignore "Self"
                    continue;
                }

                hit.transform.GetComponent<PlayerController>().Hit(this);
            }
        }

        private void Repair()
        {
            var time = Time.timeSinceLevelLoad;
            if (time - _lastRepair < 1f / this.repairPerSecond)
            {
                return;
            }

            _lastRepair = time;
            Debug.Log(string.Concat("Player ", this.playerIndex, " - Repair"));

            var dir = _mover.rotation * Vector3.forward;
            var hits = Physics.SphereCastAll(this.transform.position + (dir * 0.5f), _radius, dir, this.repairRadius, Layers.instance.tankLayer);
            for (int i = 0; i < hits.Length; i++)
            {
                var tank = hits[i].transform.GetComponent<Tank>();
                if (!ReferenceEquals(tank.player, this))
                {
                    // ignore anyone but "self"
                    continue;
                }

                tank.isLeaking = false;
            }
        }

        private void Dash()
        {
            var time = Time.time;
            if (time - _lastDash < 1f / this.dashPerSecond)
            {
                return;
            }

            _lastDash = time;
            _mover.Dash();
            Debug.Log(this.ToString() + " DASH!");
        }

        public void Bounce(Vector3 normal)
        {
            _mover.Bounce(normal);
        }

        public void Hit(PlayerController attacker)
        {
            Debug.Log(this.ToString() + " hit by " + attacker.ToString());
            var dir = (this.transform.position - attacker.transform.position);
            _mover.input += dir * this.hitPower;
            this.isHit = true;
        }

        public void Die()
        {
            this.gameObject.SetActive(false);
            Debug.Log(this.ToString() + " has died");
        }
    }
}