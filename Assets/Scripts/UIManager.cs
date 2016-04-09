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

		//public void OnClickMainMenu()
		//{
		//	SceneManager.LoadScene("Test");
		//}

  //      private void Update()
  //      {
  //          if (Input.GetButtonDown("Reload"))
  //          {
  //              SceneManager.LoadScene("testlevel_2");
  //          }
  //      }
    }
}