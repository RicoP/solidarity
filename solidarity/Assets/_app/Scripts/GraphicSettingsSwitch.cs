using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicSettingsSwitch : MonoBehaviour
{
  public RenderTexture gameTextureTarget;

  void OnGUI()
  {
    string[] names = QualitySettings.names;
    GUILayout.BeginVertical();
    for (int i = 0; i < names.Length; i++)
    {
      string name = names[i];
      if (GUILayout.Button(name))
      {
        QualitySettings.SetQualityLevel(i, true);
        float resolutionScale = 1;
        if (name == "mobile") resolutionScale = 0.5f;
        gameTextureTarget.width = (int)(Screen.width * resolutionScale);
        gameTextureTarget.height= (int)(Screen.height * resolutionScale);
      }
    }
    GUILayout.Label("Resolution " + new Vector2(gameTextureTarget.width, gameTextureTarget.height));
    GUILayout.EndVertical();
  }
}
