using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowFPS : MonoBehaviour
{
  public bool active = false;
  public float Size = 10;
  public Color color = new Color(0.0f, 0.0f, 0.5f, 1.0f);

  //http://wiki.unity3d.com/index.php?title=FramesPerSecond&_ga=2.4209459.1836638150.1576842329-1024629228.1560439099
  float fpsDeltaTime = 0.0f;
  GUIStyle style = new GUIStyle();

  float[] lastFPS = new float[100];
  int lastFPSIndex = 0;
  float median = 0;
  private float min = 999;
  private float max = 0;

  void Update()
  {
    fpsDeltaTime += (Time.unscaledDeltaTime - fpsDeltaTime) * 0.1f;
  }


  void OnGUI()
  {
    if (!active) return;

    int w = Screen.width, h = Screen.height;

    Rect rect = new Rect(0, 0, w, h * 2 / 100);
    style.alignment = TextAnchor.UpperLeft;
    style.fontSize = (int)(h * Size / 100.0f);
    style.normal.textColor = color;
    float msec = fpsDeltaTime * 1000.0f;
    float fps = 1.0f / fpsDeltaTime;

    lastFPS[lastFPSIndex++] = fps;
    if (lastFPSIndex == lastFPS.Length) {
      lastFPSIndex = 0;
      Array.Sort(lastFPS);
      median = lastFPS[lastFPS.Length / 2];

      if(median < min) { min = median; }
      if(median > max) { max = median; }
    }

    string text = string.Format("{0:0.0} ms ({1:0.} fps / {2:0.} avg / {3:0.} min / {4:0.} max)", msec, fps, median, min, max);
    GUI.Label(rect, text, style);
  }
}
