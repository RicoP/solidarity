using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
  public string[] scenes = new string[0];

  string loadingText = "Loading...";

  IEnumerator LoadYourAsyncScene()
  {
    // The Application loads the Scene in the background as the current Scene runs.
    // This is particularly good for creating loading screens.
    // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
    // a sceneBuildIndex of 1 as shown in Build Settings.


    for (int i = 0; i < scenes.Length; i++)
    {
      string scene = scenes[i];

      loadingText = "Loading " + scene;

      AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene,  LoadSceneMode.Additive);

      // Wait until the asynchronous scene fully loads
      while (!asyncLoad.isDone)
      {
        yield return null;
      }
    }

    Destroy(gameObject);
  }

  void Awake()
  {
    StartCoroutine(LoadYourAsyncScene());
  }

  public void OnGUI()
  {
    GUILayout.Label(loadingText);
  }

}
