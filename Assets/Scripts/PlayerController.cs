namespace Game
{
    using System;
    using System.Collections;
    using UnityEngine;

    [RequireComponent(typeof(PlayerMover))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class PlayerController : MonoBehaviour
    {
        private const float _velocityIsHitThresholdSqr = 0.1f;

        [Range(1, 3)]
        public int playerIndex = 1;

        [Range(0.1f, 10f)]
        public float attacksPerSecond = 1f;

        [Range(0.1f, 10f)]
        public float attackRadius = 1f;

        [Range(0.1f, 2f)]
        public float attackDuration = 0.8f;

        [Range(1f, 1000f)]
        public float hitPower = 10f;

        [Range(0.1f, 10f)]
        public float repairPerSecond = 1f;

        [Range(0.1f, 10f)]
        public float repairRadius = 1f;

        [Range(0.1f, 10f)]
        public float dashPerSecond = 1f;

        public GameObject onHitParticleSystemGO;

        [SerializeField, ReadOnly]
        private bool _isVulnerable;

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
        private string dash = string.Empty;

        public Vector3 attackDirection
        {
            get;
            private set;
        }

        public bool isHit
        {
            get;
            private set;
        }

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
            if (Mathf.Abs(hrot) > 0.5f || Mathf.Abs(vrot) > 0.5f)
            {
                Attack(hrot, vrot);
            }

            var act = Input.GetAxis(this.interact);
            if (act == 1f)
            {
                Repair();
            }

            if (Input.GetButtonUp(this.dash))
            {
                Dash();
            }
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

            //Debug.Log(this.ToString() + " reflect off wall");
            _mover.Bounce(collision.contacts[0].normal);
        }

        private void OnDisable()
        {
            PlayerList.instance.Remove(this);
        }

        private void Attack(float hrot, float vrot)
        {
            var time = Time.timeSinceLevelLoad;
            if (time - _lastAttack < 1f / this.attacksPerSecond)
            {
                return;
            }

            _lastAttack = time;
            Debug.Log(string.Concat("Player ", this.playerIndex, " - Attack"));

            var angle = Mathf.Atan2(vrot, hrot) * Mathf.Rad2Deg;
            var newAngle = Quaternion.AngleAxis(angle + 90f, Vector3.up);
            var dir = newAngle * Vector3.forward;
            this.attackDirection = dir;
            CoroutineHelper.instance.StartCoroutine(StopAttack());

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

            SoundManager.instance.PlayFx(SoundFxType.PlayerSwing);
        }

        private IEnumerator StopAttack()
        {
            yield return new WaitForSeconds(this.attackDuration);
            this.attackDirection = Vector3.zero;
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

            var hits = Physics.OverlapSphere(this.transform.position, _radius + this.repairRadius, Layers.instance.tankLayer);
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
            SoundManager.instance.PlayFx(SoundFxType.PlayerDash);
            Debug.Log(this.ToString() + " DASH!");
        }

        private IEnumerator RemoveGameObject(GameObject go, float duration)
        {
            yield return new WaitForEndOfFrame();
            Destroy(go, duration);
        }

        private void PlayParticleSystems(GameObject go)
        {
            var systems = go.GetComponentsInChildren<ParticleSystem>();
            if (systems.Length == 0)
            {
                Debug.LogError(this.ToString() + " GameObject: " + go.ToString() + " has no particle systems");
                return;
            }

            var longestDuration = systems[0].duration;
            for (int i = 0; i < systems.Length; i++)
            {
                var system = systems[i];
                system.Play();

                if (longestDuration < system.duration)
                {
                    longestDuration = system.duration;
                }
            }

            CoroutineHelper.instance.StartCoroutine(RemoveGameObject(go, longestDuration));
        }

        public void Bounce(Vector3 normal)
        {
            _mover.Bounce(normal);
        }

        public void Hit(PlayerController attacker)
        {
            if (this.onHitParticleSystemGO != null)
            {
                var systemGO = (GameObject)Instantiate(this.onHitParticleSystemGO, this.transform.position, this.transform.rotation);
                PlayParticleSystems(systemGO);
            }

            if (_isVulnerable)
            {
                Debug.Log(this.ToString() + " killed by " + attacker.ToString());
                OnDeath();
                return;
            }

            Debug.Log(this.ToString() + " hit by " + attacker.ToString());
            _mover.input += (this.transform.position - attacker.transform.position).normalized * this.hitPower;

            this.isHit = true;
            CoroutineHelper.instance.StartCoroutine(SetHitFalse());
            SoundManager.instance.PlayFx(SoundFxType.PlayerHit);
        }

        private IEnumerator SetHitFalse()
        {
            yield return new WaitForSeconds(0.1f);

            while (_mover.velocity.sqrMagnitude > _velocityIsHitThresholdSqr)
            {
                yield return new WaitForEndOfFrame();
            }

            this.isHit = false;
        }

        public void OnTankDepleted()
        {
            _isVulnerable = true;
            Debug.Log(this.ToString() + " has an empty tank, and is now vulnerable");
        }

        public void OnDeath()
        {
            Debug.Log(this.ToString() + " has died");
            CutsceneManager.instance.StartCutscene(this.gameObject);
        }
    }
}