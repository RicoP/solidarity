using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class splitModel : MonoBehaviour
{
  private Camera cam;
  public float xBegin;

  public splitModel(Camera cam, float xBegin)
  {
    this.cam = cam;
    this.xBegin = xBegin;
  }

  private const float yBegin = 0;

  void Initialize()
  {

    cam = GetComponent<Camera>();
    float width = Screen.width;
    float height = Screen.height;
    cam.rect = new Rect(xBegin, yBegin, 0.5f, 1);
  }

  private void Start()
  {
    new splitModel(cam, xBegin);
  }

  void Update()
  {
  }
}
