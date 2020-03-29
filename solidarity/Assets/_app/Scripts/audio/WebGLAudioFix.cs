using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebGLAudioFix : MonoBehaviour
{
  bool audioResumed = false;
  public void ResumeAudio()
  {
    if (!audioResumed)
    {
      var result = FMODUnity.RuntimeManager.CoreSystem.mixerSuspend();

      if (Debug.isDebugBuild && result != FMOD.RESULT.OK)
      {
        Debug.LogWarning(result);
      }

      result = FMODUnity.RuntimeManager.CoreSystem.mixerResume();

      if (Debug.isDebugBuild && result != FMOD.RESULT.OK)
      {
        Debug.LogWarning(result);
      }

      audioResumed = true;
    }
  }
}
