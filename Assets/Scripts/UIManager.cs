namespace Game
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class UIManager : MonoBehaviour
    {
        public string gameScene = "testLevel_2";

        public void OnClickStart()
        {
            SceneManager.LoadScene(this.gameScene);
        }

        public void OnClickExit()
        {
            Application.Quit();
        }
    }
}