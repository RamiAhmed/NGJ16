namespace Game
{
    using UnityEngine;

    public class PlayerMover : MonoBehaviour
    {
        [SerializeField, Range(1f, 1000f)]
        private float _acceleration = 400f;

        [Range(1f, 200f)]
        public float friction = 4f;

        [Range(1f, 90f)]
        public float angularSpeed = 15f;

#if UNITY_EDITOR

        [ReadOnly]
        public float maxSpeed;

        [ReadOnly]
        public float timeToMaxSpeed;

#endif

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

        private void Update()
        {
#if UNITY_EDITOR
            // Update values for convenience/readability
            this.maxSpeed = _acceleration / friction;
            this.timeToMaxSpeed = 2f / friction;
#endif
        }

        private void FixedUpdate()
        {
            var deltaTime = Time.deltaTime;
            var prevVelocity = this.velocity;

            this.acceleration = this.input * _acceleration * deltaTime;
            this.input = Vector3.zero;

            this.velocity -= this.velocity * this.friction * deltaTime;
            this.velocity += this.acceleration * deltaTime;

            // Velocity Verlet integration : http://lolengine.net/blog/2011/12/14/understanding-motion-in-games
            var speed = this.position + ((prevVelocity + this.velocity) / 0.5f) * deltaTime;
            speed.y = 0f;
            this.position = speed;

            if (_horizontalRot != 0f || _verticalRot != 0f)
            {
                var angle = Mathf.Atan2(_verticalRot, _horizontalRot) * Mathf.Rad2Deg;
                var newAngle = Quaternion.AngleAxis(angle + 90f, Vector3.up);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newAngle, Time.deltaTime * this.angularSpeed);
            }
        }

        public void Rotate(float horizontal, float vertical)
        {
            _horizontalRot = horizontal;
            _verticalRot = vertical;
        }
    }
}