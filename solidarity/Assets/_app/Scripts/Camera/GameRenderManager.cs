using System;
using UnityEngine;

public class GameRenderManager : MonoBehaviour
{
  private int layerMask;

  //fps calculation
  public bool active = false;
  public float FontSize = 4;
  public Color color = Color.yellow;

  //http://wiki.unity3d.com/index.php?title=FramesPerSecond&_ga=2.4209459.1836638150.1576842329-1024629228.1560439099
  GUIStyle style = new GUIStyle();

  float[] fpslist = new float[59];
  int fpslistIndex = 0;
  float median = 0;
  float min = 999;
  float max = 0;
  int checkNumber = 0;
  string guiText = "calculating...";
  public Font font;

  private GameObject renderPlane;
  private Camera gameCamera;

  public static GameRenderManager Instance { get; private set; }

  void Awake()
  {
    if (Instance != null)
    {
      Destroy(this.gameObject);
      return;
    }

    if (!font)
    {
      font = Font.CreateDynamicFontFromOSFont("Courier", 8);
    }

    style.alignment = TextAnchor.UpperLeft;
    style.normal.textColor = color;
    style.font = font;
    style.fontSize = (int)(Screen.height * FontSize / 100.0f);

    layerMask = LayerMask.NameToLayer("RenderCamera");

    Instance = this;
    var go = this.gameObject;
    DontDestroyOnLoad(this.gameObject);
    go.transform.localPosition = Vector3.zero;

    //Create MainGame Camera
    gameCamera = go.AddComponent<Camera>();
    //camera.clearFlags = CameraClearFlags.Nothing; //???
    gameCamera.clearFlags = CameraClearFlags.Depth;
    gameCamera.backgroundColor = Color.gray;
    gameCamera.orthographic = true;
    gameCamera.orthographicSize = .5f;
    gameCamera.nearClipPlane = 0.1f;
    gameCamera.farClipPlane = 2;
    gameCamera.useOcclusionCulling = false;
    gameCamera.cullingMask = 1 << layerMask;
    gameCamera.depth = float.MaxValue; //make sure this camera is rendered last.

    renderPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
    renderPlane.name = "renderPlane";
    renderPlane.transform.parent = go.transform;
    renderPlane.transform.localPosition = new Vector3(0, 0, 1);
    renderPlane.transform.localScale = new Vector3(16.0f / 9.0f, 1, 1); //fixed 16:9

    go.layer = layerMask;
    renderPlane.layer = layerMask;

    //Create Render Texture
  }

  public void OnPreRender()
  {
    const float DesiredAspectRatio = 16.0f / 9.0f; //fixed 16:9
    const float DesiredAspectRatioInv = 1.0f / DesiredAspectRatio;
    float aspect = (float)Screen.width / (float)Screen.height;

    if (aspect >= DesiredAspectRatio)
    {
      renderPlane.transform.localScale = new Vector3(DesiredAspectRatio, 1, 1);
    }
    else
    {
      renderPlane.transform.localScale = new Vector3(aspect, aspect * DesiredAspectRatioInv, 1);
    }
  }

  public void OnPostRender()
  {
    float fpsDeltaTime = Time.unscaledDeltaTime;
    float fps = 1.0f / fpsDeltaTime;

    fpslist[fpslistIndex++] = fps;
    if (fpslistIndex == fpslist.Length)
    {
      fpslistIndex = 0;
      Array.Sort(fpslist);
      median = fpslist[fpslist.Length / 2];

      //do at least 4 passes before min and max fps are reliable
      if (checkNumber > 4)
      {
        if (median < min) { min = median; }
        if (median > max) { max = median; }
        if (active)
        {
          guiText = string.Format(" {0,5:###.0} fps, {1,5:###.0} min, {2,5:###.0} max", median, min, max);
        }
      }
      else
      {
        checkNumber++;
      }
    }
  }

  public void OnGUI()
  {
    if (!active) return;
    Rect rect = new Rect(0, 0, 400, 200);
    GUI.Label(rect, guiText, style);
  }
}
