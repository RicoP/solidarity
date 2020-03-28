using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnGUI()
    {
     Texture2D tex = new Texture2D(10, 10);
      float linePosX = (Screen.width / 2);
      float lineHeight = (Screen.height);
      Color lineColor = new Color(255, 255, 255);
      GUI.DrawTexture(new Rect(linePosX, 0, 15, lineHeight), tex, ScaleMode.StretchToFill, false, 0, lineColor, 0f, 0f);
    }
}
