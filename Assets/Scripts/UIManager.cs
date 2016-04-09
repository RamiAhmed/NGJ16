using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {


	public void OnClickStart(){
		SceneManager.LoadScene("3DScene_Test1");

	}

	public void OnClickExit(){
		Application.Quit();
	}
}
