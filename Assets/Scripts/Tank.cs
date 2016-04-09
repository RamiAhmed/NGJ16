namespace Game
{
    using UnityEngine;

    [RequireComponent(typeof(Collider))]
    public class Tank : MonoBehaviour
    {
        public PlayerController player;

        [Range(10f, 1000f)]
        public float max = 100f;

        [Range(0.1f, 20f)]
        public float leakRatePerSecond = 2f;

        [ReadOnly]
        private float current = 0f;

        public bool isLeaking
        {
            get;
            set;
        }

        private void OnEnable()
        {
            this.name = string.Concat("Player ", this.player.playerIndex, " Tank");
            this.current = this.max;
        }

        private void Update()
        {
            if (!this.isLeaking || PauseManager.isPaused)
            {
                return;
            }

            this.current -= 1f / this.leakRatePerSecond;
            if (this.current <= 0f)
            {
                this.player.Die();
                this.enabled = false;
            }

            // TODO: Debug ONLY
            var frac = this.current / this.max;
            this.GetComponent<Renderer>().material.color = new Color(frac, frac, frac);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (this.isLeaking || PauseManager.isPaused)
            {
                return;
            }

            var other = collision.gameObject;
            if (((1 << other.layer) & Layers.instance.playerLayer) == 0)
            {
                // not a player
                return;
            }

            var player = other.GetComponent<PlayerController>();
            if (!player.isHit)
            {
                return;
            }

            Debug.Log(this.ToString() + " start leaking");
            var normal = (player.transform.position - this.transform.position).normalized;
            player.Bounce(normal);
            this.isLeaking = true;
        }
    }
}