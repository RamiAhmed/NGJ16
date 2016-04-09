namespace Game
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerMover))]
    [RequireComponent(typeof(PlayerController))]
    public class PlayerAnimator : MonoBehaviour
    {
        private const float moveThreshold = 0.1f;
        private const float attackDirThreshold = 0.1f;
        private const float dashDirThreshold = 0.1f;

        public string walking = "Walking";

        public string attackingUp = "AttackUp";

        public string attacking = "AttackSideways";

        public string attackingDown = "AttackDown";

        public string dashingLeft = "DashLeft";

        public string dashingRight = "DashRight";

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
            var velocity = _mover.velocity;
            var walking = velocity.sqrMagnitude > moveThreshold;
            _animator.SetBool(this.walking, walking);
            if (walking)
            {
                var left = velocity.x < 0f;
                _spriteRenderer.flipX = left;
            }

            var attackDir = _player.attackDirection;
            if (attackDir.sqrMagnitude > attackDirThreshold)
            {
                var attackSideways = Mathf.Abs(attackDir.x) > attackDirThreshold;
                _animator.SetBool(this.attacking, attackSideways);
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
            }

            var dashDir = _mover.dashVelocity;
            if (dashDir.sqrMagnitude > dashDirThreshold)
            {
                var left = dashDir.x < 0f;
                _animator.SetBool(this.dashingLeft, left);
                _animator.SetBool(this.dashingRight, !left);
            }

            _animator.SetBool(this.hit, _player.isHit);
        }
    }
}