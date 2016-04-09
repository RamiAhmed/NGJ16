namespace Game
{
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField, Range(1f, 1000f)]
        private float _acceleration = 400f;

        [Range(1f, 200f)]
        public float friction = 4f;

        [Range(1f, 90f)]
        public float angularSpeed = 15f;

        [Range(1f, 100f)]
        public float dashDistance = 10f;

#if UNITY_EDITOR

        [ReadOnly]
        public float maxSpeed;

        [ReadOnly]
        public float timeToMaxSpeed;

#endif

        private Rigidbody _rb;
        private float _horizontalRot;
        private float _verticalRot;

        public Vector3 input
        {
            get;
            set;
        }

        public Vector3 velocity
        {
            get;
            private set;
        }

        public Vector3 dashVelocity
        {
            get;
            private set;
        }

        public Vector3 acceleration
        {
            get;
            private set;
        }

        public Vector3 position
        {
            get { return this.transform.position; }
            set { this.transform.position = value; }
        }

        public Quaternion rotation
        {
            get;
            set;
        }

        private void OnEnable()
        {
            _rb = this.GetComponent<Rigidbody>();
        }

#if UNITY_EDITOR

        private void Update()
        {
            // Update values for convenience/readability
            this.maxSpeed = _acceleration / friction;
            this.timeToMaxSpeed = 2f / friction;
        }

#endif

        private void FixedUpdate()
        {
            if (PauseManager.isPaused)
            {
                return;
            }

            if (_rb != null)
            {
                _rb.angularVelocity = Vector3.zero;
            }

            var deltaTime = Time.deltaTime;
            var prevVelocity = this.velocity;

            this.acceleration = this.input * _acceleration * deltaTime;
            this.input = Vector3.zero;

            this.velocity -= this.velocity * this.friction * deltaTime;
            this.velocity += this.acceleration * deltaTime;

            if (this.dashVelocity.sqrMagnitude > 0f)
            {
                this.velocity += this.dashVelocity;
                this.dashVelocity = Vector3.zero;
            }

            // Velocity Verlet integration : http://lolengine.net/blog/2011/12/14/understanding-motion-in-games
            var speed = ((prevVelocity + this.velocity) / 0.5f);
            speed.y = 0f;

            // We set the velocity on the rigidbody so as to utilize Unity collision detection, this in turn means that we do not multiply by deltaTime as this is done internally in rigidbody
            _rb.velocity = speed;

            if (_horizontalRot != 0f || _verticalRot != 0f)
            {
                var angle = Mathf.Atan2(_verticalRot, _horizontalRot) * Mathf.Rad2Deg;
                var newAngle = Quaternion.AngleAxis(angle + 90f, Vector3.up);
                this.rotation = Quaternion.Slerp(this.rotation, newAngle, deltaTime * this.angularSpeed);
            }
        }

        public void Rotate(float horizontal, float vertical)
        {
            _horizontalRot = horizontal;
            _verticalRot = vertical;
        }

        public void Bounce(Vector3 normal)
        {
            this.velocity = Vector3.Reflect(this.velocity, normal);
        }

        public void Dash()
        {
            // Do we want dash to move in the rotation direction or the velocity direction ?
            //this.dashVelocity = (this.rotation * Vector3.forward) * this.dashDistance;
            this.dashVelocity = this.velocity.normalized * this.dashDistance;
        }
    }
}