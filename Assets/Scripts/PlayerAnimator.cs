namespace Game
{
    using UnityEngine;

    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerMover))]
    [RequireComponent(typeof(PlayerController))]
    public class PlayerAnimator : MonoBehaviour
    {
        private const float attackDirThreshold = 0.1f;

        public string walkingLeft = "WalkingLeft";

        public string walkingRight = "WalkingRight";

        public string attackingUp = "AttackUp";

        public string attackingRight = "AttackRight";

        public string attackingLeft = "AttackLeft";

        public string attackingDown = "AttackDown";

        public string dashingLeft = "DashLeft";

        public string dashingRight = "DashRight";

        public string hit = "Hit";

        private PlayerMover _mover;
        private PlayerController _player;
        private Animator _animator;

        private void OnEnable()
        {
            _mover = this.GetComponent<PlayerMover>();
            _player = this.GetComponent<PlayerController>();
            _animator = this.GetComponent<Animator>();
        }

        private void Update()
        {
            var velocity = _mover.velocity;
            if (velocity.sqrMagnitude > 0f)
            {
                var left = velocity.x < 0f;
                _animator.SetBool(this.walkingLeft, left);
                _animator.SetBool(this.walkingLeft, !left);
            }

            var attackDir = _player.attackDirection;
            if (attackDir.sqrMagnitude > 0f)
            {
                if (Mathf.Abs(attackDir.x) > attackDirThreshold)
                {
                    // sideways attacks are prioritized first
                    var left = attackDir.x < 0f;
                    _animator.SetBool(this.attackingLeft, left);
                    _animator.SetBool(this.attackingRight, !left);
                }
                else if (Mathf.Abs(attackDir.z) > attackDirThreshold)
                {
                    var up = attackDir.z > 0f;
                    _animator.SetBool(this.attackingUp, up);
                    _animator.SetBool(this.attackingDown, !up);
                }
            }

            var dashDir = _mover.dashVelocity;
            if (dashDir.sqrMagnitude > 0f)
            {
                var left = dashDir.x < 0f;
                _animator.SetBool(this.dashingLeft, left);
                _animator.SetBool(this.dashingRight, !left);
            }

            _animator.SetBool(this.hit, _player.isHit);
        }
    }
}