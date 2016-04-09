namespace Game
{
    using System;
    using UnityEngine;

    public class PlayerController : MonoBehaviour
    {
        public int playerIndex = 1;

        private PlayerMover _mover;

        private string horizontal = string.Empty;
        private string vertical = string.Empty;

        private void OnEnable()
        {
            _mover = this.GetComponent<PlayerMover>();
            if (_mover == null)
            {
                throw new ArgumentNullException("_mover");
            }

            this.horizontal = string.Concat("Horizontal_", this.playerIndex);
            this.vertical = string.Concat("Vertical_", this.playerIndex);
        }

        private void Update()
        {
            var horz = Input.GetAxis(this.horizontal);
            var vert = Input.GetAxis(this.vertical);
            if (horz != 0f || vert != 0f)
            {
                _mover.input = new Vector3(horz, 0f, -vert);
            }
        }
    }
}