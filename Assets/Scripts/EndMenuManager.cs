namespace Game
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class EndMenuManager : MonoBehaviour
    {
        public static EndMenuManager instance { get; private set; }

        public string mainMenuScene = "MainMenu";

        private void OnEnable()
        {
            if (instance != null)
            {
                Debug.LogWarning(this.ToString() + " another EndMenuManager has already been registered, destroying this one");
                Destroy(this.gameObject);
                return;
            }

            instance = this;

            // start by disabling child
            this.transform.GetChild(0).gameObject.SetActive(false);
        }

        public void Activate()
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
            PauseManager.isPaused = true;
            ScoreboardManager.instance.SetScoreText();
        }

        public void Restart()
        {
            // reload current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Quit()
        {
            SceneManager.LoadScene(this.mainMenuScene);
        }
    }
}