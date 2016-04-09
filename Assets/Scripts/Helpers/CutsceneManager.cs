namespace Game
{
    using System.Collections;
    using UnityEngine;

    public class CutsceneManager : MonoBehaviour
    {
        public static CutsceneManager instance { get; private set; }

        //public Texture2D texture;

        [Range(0.01f, 0.9f)]
        public float slowTime = 0.2f;

        [Range(1f, 60f)]
        public float durationInSeconds = 3f;

        private bool _showing;

        //private Vector3 _focus;
        private GameObject _go;

        private void OnEnable()
        {
            //if (this.texture == null)
            //{
            //    Debug.LogWarning(this.ToString() + " is missing an overlay texture");
            //}

            if (instance != null)
            {
                Debug.LogWarning(this.ToString() + " another CutsceneManager has already been registered");
                Destroy(this);
                return;
            }

            instance = this;
        }

        //private void OnGUI()
        //{
        //    if (!_showing || this.texture == null)
        //    {
        //        return;
        //    }

        //    var width = Screen.width;
        //    var height = Screen.height;
        //    var pos = Camera.main.WorldToScreenPoint(_focus);
        //    pos.y = height - pos.y;
        //    GUI.DrawTexture(new Rect(pos.x - (width * 0.5f), pos.y - (height * 0.5f), width, height), this.texture);
        //}

        public void StartCutscene(GameObject go)
        {
            if (_showing)
            {
                return;
            }

            _go = go;
            //_focus = go.transform.position;
            _showing = true;
            Time.timeScale = this.slowTime;

            StartCoroutine(EndCutscene());
        }

        private IEnumerator EndCutscene()
        {
            yield return new WaitForSeconds(this.durationInSeconds);
            Time.timeScale = 1f;
            _showing = false;
            _go.SetActive(false);
        }
    }
}