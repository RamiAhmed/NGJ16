namespace Game
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(Animator))]
    public class PlayerAnimator : MonoBehaviour
    {
        private const float moveThreshold = 0.1f;
        private const float attackDirThreshold = 0.1f;
        private const float dashDirThreshold = 0.1f;

        public string walking = "Walking";

        public string attackingSideways = "AttackSideways";

        public string attackingUp = "AttackUp";

        public string attackingDown = "AttackDown";

        public string dashingSideways = "DashSideways";

        public string dashingUp = "DashUp";

        public string dashingDown = "DashDown";

        public string hit = "Hit";

        private PlayerMover _mover;
        private PlayerController _player;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        private void OnEnable()
        {
            _spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
            if (_spriteRenderer == null)
            {
                throw new ArgumentNullException("_spriteRenderer");
            }

            _mover = this.GetComponent<PlayerMover>();
            _player = this.GetComponent<PlayerController>();
            _animator = this.GetComponent<Animator>();
        }

        private void Update()
        {
            HandleDashing();
            HandleWalking();
            HandleAttacking();

            _animator.SetBool(this.hit, _player.isHit);
        }

        private bool HandleDashing()
        {
            var dashDir = _mover.lastDashDirection;
            if (dashDir.sqrMagnitude > dashDirThreshold)
            {
                var dashSideways = Mathf.Abs(dashDir.x) > dashDirThreshold;
                _animator.SetBool(this.dashingSideways, dashSideways);
                if (dashSideways)
                {
                    var left = dashDir.x < 0f;
                    _spriteRenderer.flipX = left;
                }
                else if (Mathf.Abs(dashDir.z) > dashDirThreshold)
                {
                    var up = dashDir.z > 0f;
                    _animator.SetBool(this.dashingUp, up);
                    _animator.SetBool(this.dashingDown, !up);
                }

                return true;
            }

            _animator.SetBool(this.dashingSideways, false);
            _animator.SetBool(this.dashingUp, false);
            _animator.SetBool(this.dashingDown, false);
            return false;
        }

        private bool HandleAttacking()
        {
            var attackDir = _player.attackDirection;
            if (attackDir.sqrMagnitude > attackDirThreshold)
            {
                var attackSideways = Mathf.Abs(attackDir.x) > attackDirThreshold;
                _animator.SetBool(this.attackingSideways, attackSideways);
                if (attackSideways)
                {
                    // sideways attacks are prioritized first
                    var left = attackDir.x < 0f;
                    _spriteRenderer.flipX = left;
                    // TODO: This needs to prevent "walking flipping" for at least a second or so
                }
                else if (Mathf.Abs(attackDir.z) > attackDirThreshold)
                {
                    var up = attackDir.z > 0f;
                    _animator.SetBool(this.attackingUp, up);
                    _animator.SetBool(this.attackingDown, !up);
                }

                return true;
            }

            _animator.SetBool(this.attackingSideways, false);
            _animator.SetBool(this.attackingUp, false);
            _animator.SetBool(this.attackingDown, false);
            return false;
        }

        private bool HandleWalking()
        {
            var velocity = _mover.velocity;
            var walking = velocity.sqrMagnitude > moveThreshold && _mover.dashVelocity.sqrMagnitude < dashDirThreshold;
            _animator.SetBool(this.walking, walking);
            if (walking)
            {
                var left = velocity.x < 0f;
                _spriteRenderer.flipX = left;
            }

            return walking;
        }
    }
}