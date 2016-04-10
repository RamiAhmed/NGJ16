namespace Game
{
    using System.Collections.Generic;
    using UnityEngine;

    public class PlayerList : MonoBehaviour
    {
        public static PlayerList instance { get; private set; }

        private HashSet<PlayerController> _players = new HashSet<PlayerController>();

        private void OnEnable()
        {
            if (instance != null)
            {
                Debug.LogWarning(this.ToString() + " another PlayerList has already been registered, destroying this one");
                Destroy(this);
                return;
            }

            instance = this;
        }

        public void Add(PlayerController pc)
        {
            _players.Add(pc);
        }

        public bool Remove(PlayerController pc)
        {
            var removed = _players.Remove(pc);
            if (removed)
            {
                if (_players.Count <= 1)
                {
                    var endMenu = EndMenuManager.instance;
                    if (endMenu != null)
                    {
                        endMenu.Activate();
                    }
                    else
                    {
                        Debug.LogWarning(this.ToString() + " could not find the EndMenuManager");
                    }
                }
            }

            return removed;
        }
    }
}