using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Renderer))]
public class Goal : MonoBehaviour
{
    static public bool goalMet = false;

    void OnTriggerEnter(Collider other)
    {
        // when the trigger is hit by something
        // check to see if it's a Projectile 
        Projectile proj = other.GetComponent<Projectile>();
        if (proj != null)
        {
            // if so, set goalMet = true
            goalMet = true;
            UnlockednewLevel();
            LoadNextLevel();
        }

        // also set the alpha of the color of higher opacity
        Material mat = GetComponent<Renderer>().material;
        Color c = mat.color;
        c.a = 0.75f;
        mat.color = c;
    }

    void UnlockednewLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save();
        }
    }

    void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}