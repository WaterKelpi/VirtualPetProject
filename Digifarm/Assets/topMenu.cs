using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class topMenu : MonoBehaviour {
	void Awake(){
		//Camera.main.aspect = 9/16;
	}
	public void startGame(){
		SceneManager.LoadScene("Farm");
	}
	public void clearSave(){
		saveLoadManager.ClearPlayer();
	}
}
