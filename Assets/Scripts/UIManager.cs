namespace Game
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class UIManager : MonoBehaviour
    {
        public string gameScene = "3DScene_Test1";

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