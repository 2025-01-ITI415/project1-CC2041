using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject[] panels; // The scrolling backgrounds
    public float scrollSpeed = 5f; // Positive value to move downward

    private float panelHeight; // Height of each panel
    private float depth; // Depth of panels (that is, pos.z)

    private void Start()
    {
        panelHeight = panels[0].transform.localScale.y;
        depth = panels[0].transform.position.z;

        // Set initial positions of panels
        panels[0].transform.position = new Vector3(0, 0, depth);
        panels[1].transform.position = new Vector3(0, panelHeight, depth);
    }

    private void Update()
    {
        float tY = Time.time * scrollSpeed % panelHeight - (panelHeight * 0.5f);

        // Position panels[0]
        panels[0].transform.position = new Vector3(0, tY, depth);
        // Position panels[1] to make a continuous background
        if (tY <= 0)
        {
            panels[1].transform.position = new Vector3(0, tY + panelHeight, depth);
        }
        else
        {
            panels[1].transform.position = new Vector3(0, tY - panelHeight, depth);
        }
    }
}
