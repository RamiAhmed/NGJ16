namespace Game
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(Animator))]
    public class TankAnimator : MonoBehaviour
    {
        public string leaking = "Leaking";
        public string levels = "Levels";
        public string patched = "Patched";

        private Tank _tank;
        private Animator _animator;

        private void OnEnable()
        {
            _tank = this.GetComponent<Tank>();
            if (_tank == null)
            {
                throw new ArgumentNullException("_tank");
            }

            _animator = this.GetComponent<Animator>();
        }

        private void Update()
        {
            _animator.SetBool(this.leaking, _tank.isLeaking);
            _animator.SetBool(this.patched, !_tank.isLeaking && _tank.current < _tank.max);
            _animator.SetFloat(this.levels, _tank.current);
        }
    }
}