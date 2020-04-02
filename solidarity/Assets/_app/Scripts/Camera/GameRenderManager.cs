using System;
using UnityEngine;

public class GameRenderManager : MonoBehaviour
{
  public const float renderScaleMax = 4;
  public const float renderScaleMin = .1f;

  public RenderTexture renderTexture;
  public Material renderTextureMaterial;
  [Range(renderScaleMin, renderScaleMax)]
  public float renderScale = 1;
  public float fpsTarget = 60;

  //fps calculation
  public bool showGUI = true;
  public float FontSize = 4;
  public Color color = Color.yellow;

  //http://wiki.unity3d.com/index.php?title=FramesPerSecond&_ga=2.4209459.1836638150.1576842329-1024629228.1560439099
  GUIStyle style = new GUIStyle();

  float[] fpslist = new float[59];
  int fpslistIndex = 0;
  float median = 0;
  float min = float.NaN;
  float max = float.NaN;
  string guiText = "calculating...";
  public Font font;

  private GameObject renderPlane;
  private Camera gameCamera;
  private int layerMask;
  private bool invalidated = true;


  public static GameRenderManager Instance { get; private set; }

  void Awake()
  {
    if (Instance != null)
    {
      Destroy(this.gameObject);
      return;
    }

    style.alignment = TextAnchor.UpperLeft;
    style.normal.textColor = color;
    style.fontSize = (int)(Screen.height * FontSize / 100.0f);

    layerMask = LayerMask.NameToLayer("RenderCamera");
    Instance = this;

    DontDestroyOnLoad(this.gameObject);
    this.gameObject.transform.localPosition = Vector3.zero;
    this.gameObject.layer = layerMask;

    //Create MainGame Camera
    gameCamera = this.gameObject.AddComponent<Camera>();
    gameCamera.clearFlags = CameraClearFlags.Color;
    gameCamera.backgroundColor = Color.black;
    gameCamera.orthographic = true;
    gameCamera.orthographicSize = .5f;
    gameCamera.nearClipPlane = 0.1f;
    gameCamera.farClipPlane = 2;
    gameCamera.useOcclusionCulling = false;
    gameCamera.cullingMask = 1 << layerMask;
    gameCamera.depth = float.MaxValue; //make sure this camera is rendered last.

    renderPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
    renderPlane.name = "renderPlane";
    renderPlane.transform.parent = this.gameObject.transform;
    renderPlane.transform.localPosition = new Vector3(0, 0, 1);

    if (renderPlane.TryGetComponent(out Collider collider))
    {
      Destroy(collider);
    }

    renderPlane.GetComponent<MeshRenderer>().material = renderTextureMaterial;
    renderPlane.layer = layerMask;

    RefreshRenderTexture();
  }

  private void RefreshRenderTexture()
  {
    renderTexture.Release();
    renderTexture.width = Width();
    renderTexture.height = Height();

    fpslistIndex = 0;
    min = float.NaN;
    max = float.NaN;

    invalidated = false;
  }

  public int Width()
  {
    //Fixed ingame aspect ratio
    return (int)(renderScale * (Screen.height * 16) / 9);
  }

  public int Height()
  {
    return (int)(renderScale * Screen.height);
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

    if (renderTexture.width != Width()) invalidated = true;
    if (renderTexture.height != Height()) invalidated = true;
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

      float newRenderScale = renderScale;
      if (median - fpsTarget > 5) { newRenderScale += 0.1f; } //if there is 5 fps buffer, increase resolution
      if (median - fpsTarget < 0) { newRenderScale -= 0.1f; } //if we dip bellow 60, decrease the reslution

      newRenderScale = Mathf.Clamp(newRenderScale, renderScaleMin, renderScaleMax);

      if (newRenderScale != renderScale)
      {
        renderScale = newRenderScale;
        invalidated = true;
      }

      if (float.IsNaN(min) || fps < min) { min = fps; }
      if (float.IsNaN(max) || fps > max) { max = fps; }
      if (showGUI)
      {
        guiText = string.Format(" {0,5:###.0} fps, {1,5:###.0} avg, {2,5:###.0} min, {3,5:###.0} max, {4,5:###.0} scale", fps, median, min, max, renderScale);
      }
    }

    if (invalidated)
    {
      RefreshRenderTexture();
    }
  }

  //called when one of the fields changes
  public void OnValidate()
  {
    invalidated = true;
  }

  public void OnGUI()
  {
    if (showGUI)
    {
      Rect rect = new Rect(0, 0, 400, 200);
      GUI.Label(rect, guiText, style);
    }
  }
}
