using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Penguin : MonoBehaviour
{
    static public Penguin S; // Singleton

    [Header("Set in Inspector")]
    public float penguinSpeed = 30;
    public float rollMult = -20; // Controls how much the penguin tilts when turning
    public float tiltMult = 10; // Controls forward-backward tilting when moving up/down
    private float originalSpeed; // Corrected the name to match
    public float gameRestartDelay = 2f;

    private GameObject lastTriggerGo = null;

    void Start()
    {
        if (S == null)
        {
            S = this; // Set the Singleton
        }
       
        originalSpeed = penguinSpeed; // Store the initial speed correctly
    }

    void Update()
    {
        // Get input for movement
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        // Move the penguin
        Vector3 pos = transform.position;
        pos.x += xAxis * penguinSpeed * Time.deltaTime;
        pos.y += yAxis * penguinSpeed * Time.deltaTime;
        transform.position = pos;

        // Rotate the penguin for a rolling effect
        transform.rotation = Quaternion.Euler(yAxis * tiltMult, 0, -xAxis * rollMult);
    }

    public void applySlow(float slowMultiplier, float duration)
    {
        penguinSpeed *= slowMultiplier; // Reduce speed
        Invoke("ResetSpeed", duration); // Reset after duration
    }

    void ResetSpeed()
    {
        penguinSpeed = originalSpeed; // Restore to original speed
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject go = other.gameObject;

        if (go == lastTriggerGo)
        {
            return;
        }
        lastTriggerGo = go;

        if (go.tag == "Obstacle")
        {
            Destroy(go);
        }
        else if (go.tag == "PowerUp")
        {
            Destroy(go);
        }
    }
}