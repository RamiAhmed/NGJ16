namespace Game
{
    using UnityEngine;

    public class CoroutineHelper : MonoBehaviour
    {
        public static CoroutineHelper instance { get; private set; }

        private void OnEnable()
        {
            if (instance != null)
            {
                Debug.LogWarning(this.ToString() + " another CoroutineHelper has already been registered, destroyin this one");
                Destroy(this, 0.1f);
                return;
            }

            instance = this;
        }
    }
}