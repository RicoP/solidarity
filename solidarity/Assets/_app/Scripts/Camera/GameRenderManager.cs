using UnityEngine;

public class GameRenderManager : MonoBehaviour
{
  public static GameRenderManager Instance { get; private set; }

  void Awake()
  {
    if (Instance != null)
    {
      Destroy(this.gameObject);
      return;
    }

    Instance = this;
    DontDestroyOnLoad(this.gameObject);
    //Create MainGame Camera
    var cameraGo = new GameObject("Game Camera", typeof(Camera));
    cameraGo.transform.parent = this.transform;
    cameraGo.transform.localPosition = Vector3.zero;

    var camera = cameraGo.GetComponent<Camera>();


  }

  void Update()
  {

  }
}
