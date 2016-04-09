namespace Game
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(PlayerMover))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class PlayerController : MonoBehaviour
    {
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

        private PlayerMover _mover;
        private float _lastAttack;
        private float _lastRepair;
        private float _radius;

        private string horizontalPos = string.Empty;
        private string verticalPos = string.Empty;
        private string horizontalRot = string.Empty;
        private string verticalRot = string.Empty;
        private string interact = string.Empty;

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

            _radius = this.GetComponent<SphereCollider>().radius;
        }

        private void Update()
        {
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

            // TODO: DEBUG ONLY
            this.transform.rotation = _mover.rotation;
        }

        public void Attack()
        {
            var time = Time.timeSinceLevelLoad;
            if (time - _lastAttack < 1f / this.attacksPerSecond)
            {
                return;
            }

            _lastAttack = time;
            Debug.Log(string.Concat("Player ", this.playerIndex, " - Attack"));

            var hits = Physics.SphereCastAll(this.transform.position, _radius, _mover.rotation.eulerAngles, this.attackRadius, Layers.instance.playerLayer);
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

        public void Repair()
        {
            var time = Time.timeSinceLevelLoad;
            if (time - _lastRepair < 1f / this.repairPerSecond)
            {
                return;
            }

            _lastRepair = time;
            Debug.Log(string.Concat("Player ", this.playerIndex, " - Repair"));
        }

        public void Hit(PlayerController attacker)
        {
            var dir = (this.transform.position - attacker.transform.position);
            _mover.input += dir * this.hitPower;
        }
    }
}