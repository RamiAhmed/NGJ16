namespace Game
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class UIManager : MonoBehaviour
    {
        public string gameScene = "testlevel_2";

        public void OnClickStart()
        {
            SceneManager.LoadScene(this.gameScene);
        }

        public void OnClickExit()
        {
            Application.Quit();
        }

        private void Update()
        {
            if (Input.GetButtonDown("Reload"))
            {
                SceneManager.LoadScene("testlevel_2");
            }
        }
    }
}