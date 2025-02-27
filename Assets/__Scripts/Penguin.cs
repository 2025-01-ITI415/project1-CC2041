using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Penguin : MonoBehaviour
{
    static public Penguin S; // Singleton

    [Header("Set in Inspector")]
    public float speed = 30;
    public float rollMult = -20; // Controls how much the penguin tilts when turning
    public float tiltMult = 10; // Controls forward-backward tilting when moving up/down
    private float originalSpeed;
    public float gameRestartDelay = 2f;

    private GameObject lastTriggerGo = null;

    void Start()
    {
        if (S == null)
        {
            S = this; // Set the Singleton
        }
        else
        {
            Debug.LogError("Penguin.Awake() - Attempted to assign second Penguin.S!");
        }
        originalSpeed = speed; // store the inital speed
    }

    void Update()
    {
        // Get input for movement
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        // Move the penguin
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        // Rotate the penguin for a rolling effect
        transform.rotation = Quaternion.Euler(yAxis * tiltMult, 0, -xAxis * rollMult);

    }

    public void applySlow(float slowMultiplier, float durration)
    {
        speed *= slowMultiplier; // reduce speed
        Invoke("ResetSpeed", durration); // reset after duration
    }

    void ResetSpeed()
    {
        speed = originalSpeed; // restore to original
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
            Debug.Log("Penguin hit an obstacle!");
            Destroy(go);
        }
        else if (go.tag == "PowerUp")
        {
            Debug.Log("Penguin collected a power-up!");
            Destroy(go);
        }
    }
}
