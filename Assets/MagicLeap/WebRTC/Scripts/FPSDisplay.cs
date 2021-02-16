using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    public Text textMesh;
    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval

    void Update()
    {
        accum += Time.timeScale / Time.deltaTime;
        ++frames;
        textMesh.text = (accum / frames).ToString();
    }
}
