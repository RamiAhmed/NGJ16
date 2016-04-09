namespace Game
{
    using UnityEngine;

    public class Layers : MonoBehaviour
    {
        public static Layers instance;

        public LayerMask playerLayer;
        public LayerMask tankLayer;

        private void OnEnable()
        {
            if (instance != null)
            {
                Debug.LogWarning(this.ToString() + " another Layers has already been registered, destroying this one");
                Destroy(this);
                return;
            }

            instance = this;
        }
    }
}