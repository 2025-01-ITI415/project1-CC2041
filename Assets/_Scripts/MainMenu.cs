using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour{
    
public void PlayGame(){
        SceneManager.LoadScene("Level 1"); // will possibly add more in the future
    }
public void QuitGame(){
    Debug.Log("Quit");
    Application.Quit();
}
}
    
