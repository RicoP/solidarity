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
    Debug.Log("Awake");
    if (Instance != null)
    {
      Destroy(this.gameObject);
      return;
    }

    int i = 0;
    Debug.Log("Awake " + (i++));
    
    Debug.Log("Awake " + (i++));
    style.alignment = TextAnchor.UpperLeft;
    style.normal.textColor = color;
    style.fontSize = (int)(Screen.height * FontSize / 100.0f);

    Debug.Log("Awake " + (i++));
    layerMask = LayerMask.NameToLayer("RenderCamera");

    Debug.Log("Awake " + (i++));
    Instance = this;
    Debug.Log("Awake " + (i++));
    var go = this.gameObject;
    Debug.Log("Awake " + (i++));
    DontDestroyOnLoad(this.gameObject);
    Debug.Log("Awake " + (i++));
    go.transform.localPosition = Vector3.zero;
    Debug.Log("Awake " + (i++));

    //Create MainGame Camera
    Debug.Log("Awake " + (i++));
    gameCamera = go.AddComponent<Camera>();
    //camera.clearFlags = CameraClearFlags.Nothing; //???
    Debug.Log("Awake " + (i++));
    gameCamera.clearFlags = CameraClearFlags.Color;
    Debug.Log("Awake " + (i++));
    gameCamera.backgroundColor = Color.black;
    Debug.Log("Awake " + (i++));
    gameCamera.orthographic = true;
    Debug.Log("Awake " + (i++));
    gameCamera.orthographicSize = .5f;
    Debug.Log("Awake " + (i++));
    gameCamera.nearClipPlane = 0.1f;
    Debug.Log("Awake " + (i++));
    gameCamera.farClipPlane = 2;
    Debug.Log("Awake " + (i++));
    gameCamera.useOcclusionCulling = false;
    Debug.Log("Awake " + (i++));
    gameCamera.cullingMask = 1 << layerMask;
    Debug.Log("Awake " + (i++));
    gameCamera.depth = float.MaxValue; //make sure this camera is rendered last.
    Debug.Log("Awake " + (i++));

    renderPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
    Debug.Log("Awake " + (i++));
    renderPlane.name = "renderPlane";
    Debug.Log("Awake " + (i++));
    renderPlane.transform.parent = go.transform;
    Debug.Log("Awake " + (i++));
    renderPlane.transform.localPosition = new Vector3(0, 0, 1);
    Debug.Log("Awake " + (i++));
    renderPlane.GetComponent<Collider>().enabled = false;
    Debug.Log("Awake " + (i++));
    renderPlane.GetComponent<MeshRenderer>().material = renderTextureMaterial;
    Debug.Log("Awake " + (i++));

    go.layer = layerMask;
    Debug.Log("Awake " + (i++));
    renderPlane.layer = layerMask;

    //Create Render Texture
    Debug.Log("Awake " + (i++));
    RefreshRenderTexture();
    Debug.Log("Awake end");

  }

  private void RefreshRenderTexture()
  {
    Debug.Log("RefreshRenderTexture");

    renderTexture.Release();
    renderTexture.width = Width();
    renderTexture.height = Height();

    Debug.Log(String.Format("New resolution: {0:.0}, {1:.0}", renderTexture.width, renderTexture.height));

    fpslistIndex = 0;
    min = float.NaN;
    max = float.NaN;

    invalidated = false;
    RerenderAllCameras();
    Debug.Log("RefreshRenderTexture end");
  }

  /// <summary>
  /// force all cameras to render so we don't have
  /// flashing images after a Rendertexture resize.
  /// </summary>
  private void RerenderAllCameras()
  {
    Debug.Log("RerenderAllCameras");

    Camera[] cameras = GameObject.FindObjectsOfType<Camera>();

    for (int i = 0; i < cameras.Length; i++)
    {
      Camera camera = cameras[i];
      if (camera != this.gameCamera)
      {
        camera.Render();
      }
    }
    Debug.Log("RerenderAllCameras end");

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
    Debug.Log("OnPreRender");

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

    if (invalidated)
    {
      RefreshRenderTexture();
      invalidated = false;
    }

    Debug.Log("OnPreRender end");

  }

  public void OnPostRender()
  {
    Debug.Log("OnPostRender");

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

    Debug.Log("OnPostRender end");

  }

  //called when one of the fields changes
  public void OnValidate()
  {
    //Debug.Log("OnValidate()");
    invalidated = true;
  }

  public void OnGUI()
  {
    if (!showGUI) return;
    Rect rect = new Rect(0, 0, 400, 200);
    //GUI.sca
    //GUILayout.sca
    GUI.Label(rect, guiText, style);

  }
}
