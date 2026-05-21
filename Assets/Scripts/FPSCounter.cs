using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();
        Rect rect = new Rect(10, 10, w, h * 0.02f);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = 24;
        style.normal.textColor = Color.white;

        float fps = 1.0f / deltaTime;
        float ms = deltaTime * 1000.0f;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", ms, fps);

        GUI.Label(rect, text, style);
    }
}
