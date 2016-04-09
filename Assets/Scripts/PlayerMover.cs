namespace Game
{
    using UnityEngine;

    public class PlayerMover : MonoBehaviour
    {
        [SerializeField]
        private float _acceleration = 400f;

        public float friction = 4f;

#if UNITY_EDITOR

        [ReadOnly]
        public float maxSpeed;

        [ReadOnly]
        public float timeToMaxSpeed;

#endif

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
        }
    }
}