using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private float deltaTime = 0.0f;
    public TextMeshProUGUI fpsText;

    void Update()
    {
        // Calculate the time it took to render the last frame
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

        // Calculate FPS and display it in the console
        float fps = 1.0f / deltaTime;
        fpsText.text = (fps).ToString();
        Debug.Log("FPS: " + Mathf.Round(fps));
    }
}