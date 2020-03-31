using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UnityToolbarExtender.Examples
{
  static class ToolbarStyles
  {
    public static readonly GUIStyle commandButtonStyle;

    static ToolbarStyles()
    {
      commandButtonStyle = new GUIStyle("Command")
      {
        fontSize = 16,
        alignment = TextAnchor.MiddleCenter,
        imagePosition = ImagePosition.ImageAbove,
        fontStyle = FontStyle.Bold
      };
    }
  }

  [InitializeOnLoad]
  public class SceneSwitchLeftButton
  {
    private static GUIContent contentActive;
    private static GUIContent contentInactive;

    static SceneSwitchLeftButton()
    {
      var startGameIconActivated = EditorGUIUtility.IconContent(@"winbtn_mac_max").image;
      var startGameIconDeactivated = EditorGUIUtility.IconContent(@"winbtn_mac_inact").image;
      contentActive = new GUIContent(null, startGameIconActivated, "Start Game!");
      contentInactive = new GUIContent(null, startGameIconDeactivated, "Game is already running");
      ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
    }

    static void OnToolbarGUI()
    {
      var running = EditorApplication.isPlaying;
      var buttonContent = running ? contentInactive : contentActive;

      if (GUILayout.Toggle(running, buttonContent, ToolbarStyles.commandButtonStyle))
      {
        if (!running)
        {
          SceneHelper.StartScene("Assets/_app/Scenes/loadscenes.unity");
        }
      }
    }
  }

  static class SceneHelper
  {
    static string sceneToOpen;

    public static void StartScene(string scene)
    {
      if (EditorApplication.isPlaying)
      {
        EditorApplication.isPlaying = false;
      }

      sceneToOpen = scene;
      EditorApplication.update += OnUpdate;
    }

    static void OnUpdate()
    {
      if (sceneToOpen == null ||
          EditorApplication.isPlaying || EditorApplication.isPaused ||
          EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
      {
        return;
      }

      EditorApplication.update -= OnUpdate;

      if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
      {
        EditorSceneManager.OpenScene(sceneToOpen);
        EditorApplication.isPlaying = true;
      }
      sceneToOpen = null;
    }
  }
}
