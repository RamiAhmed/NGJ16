﻿#pragma warning disable 0414

namespace Game
{
    using UnityEngine;

    [RequireComponent(typeof(Collider))]
    public class Tank : MonoBehaviour
    {
        public PlayerController player;

        [Range(10f, 1000f)]
        public float max = 100f;

        public TankSeverity[] severityLevels = new TankSeverity[0];

        [SerializeField, ReadOnly]
        private float current = 0f;

        [SerializeField, ReadOnly]
        private int _hits = 0;

#if UNITY_EDITOR

        [SerializeField, ReadOnly]
        private float _currentLeakRate;

#endif

        [SerializeField, ReadOnly]
        private ParticleSystem _currentParticles;

        private bool _isLeaking;

        public bool isLeaking
        {
            get
            {
                return _isLeaking;
            }

            set
            {
                _isLeaking = value;
                if (!_isLeaking && _currentParticles != null)
                {
                    // if no longer leaking and a particle system is playing, stop the system
                    _currentParticles.Stop();
                }
            }
        }

        private void OnEnable()
        {
            if (this.severityLevels.Length == 0)
            {
                Debug.LogError(this.ToString() + " must have at least one severity level");
                Destroy(this.gameObject, 0.01f);
                return;
            }

            this.name = string.Concat("Player ", this.player.playerIndex, " Tank");
            this.current = this.max;
        }

        private void Update()
        {
            if (!this.isLeaking || PauseManager.isPaused)
            {
                return;
            }

            var leakRate = this.severityLevels[this.severityLevels.Length - 1].leakRatePerSecond;
            for (int i = 0; i < this.severityLevels.Length; i++)
            {
                var severity = this.severityLevels[i];
                if (severity.hits != _hits)
                {
                    continue;
                }

                leakRate = severity.leakRatePerSecond;
                if (severity.particles != null)
                {
                    PlayParticleSystem(severity.particles);
                }
            }

#if UNITY_EDITOR
            _currentLeakRate = leakRate;
#endif

            this.current -= 1f / leakRate;
            if (this.current <= 0f)
            {
                this.player.OnTankDepleted();
                this.enabled = false;
                if (_currentParticles != null)
                {
                    _currentParticles.Stop();
                }
            }

            // TODO: Debug ONLY
            var frac = this.current / this.max;
            this.GetComponent<Renderer>().material.color = new Color(frac, frac, frac);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (PauseManager.isPaused)
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

            var normal = (player.transform.position - this.transform.position).normalized;
            player.Bounce(normal);

            _hits++;
            if (!this.isLeaking)
            {
                this.isLeaking = true;
                Debug.Log(this.ToString() + " start leaking");
            }
            else
            {
                Debug.Log(this.ToString() + " hits == " + _hits);
            }
        }

        private void PlayParticleSystem(ParticleSystem system)
        {
            if (ReferenceEquals(_currentParticles, system) && system.isPlaying)
            {
                // already playing the given system, so do not change or start anew
                return;
            }

            if (_currentParticles != null)
            {
                _currentParticles.Stop();
            }

            _currentParticles = system;
            _currentParticles.Play();
        }
    }
}