using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextScene : MonoBehaviour
{

  public string sceneName;

  void Start()
  {

  }

  void Update()
  {
    if (Input.anyKeyDown)
    {
      LoadTheNextScene();
    }
  }

  void LoadTheNextScene()
  {
    SceneManager.LoadScene(sceneName);
  }
}
