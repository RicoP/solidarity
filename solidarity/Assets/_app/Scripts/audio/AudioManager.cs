using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{

  static AudioManager instance;

  [FMODUnity.EventRef]
  [SerializeField]
  private string musicEvent;

  FMOD.Studio.EventInstance musicInstance;

  FMOD.Studio.VCA musicVCA, sfxVCA;

  [SerializeField]
  [Range(-80f, 10f)]
  private float musicVolume, sfxVolume = 0f;

  void Awake()
  {
    if (instance == null)
    {
      instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else if (instance != this)
      Destroy(gameObject);
  }

  void Start()
  {
    GetVCAs();
    CreateMusicInstance();
    StartMusicInstance();
  }

  void Update()
  {
    SetVcaVolumes();
  }

  private void CreateMusicInstance()
  {
    musicInstance = RuntimeManager.CreateInstance(musicEvent);
  }

  private void StartMusicInstance()
  {
    CheckResult(musicInstance.start());
  }

  private void GetVCAs()
  {
    musicVCA = RuntimeManager.GetVCA("vca:/Music");

    if (!musicVCA.isValid())
    {
      if (Debug.isDebugBuild)
      {
        Debug.Log("Could not get music vca. Path wrong?");
      }
    }

    sfxVCA = RuntimeManager.GetVCA("vca:/SFX");

    if (!sfxVCA.isValid())
    {
      if (Debug.isDebugBuild)
      {
        Debug.Log("Could not get sfx vca. Path wrong?");
      }
    }
  }

  private void SetVcaVolumes()
  {
    musicVCA.setVolume(dBtoLinear(musicVolume));
    sfxVCA.setVolume(dBtoLinear(sfxVolume));
  }

  float dBtoLinear(float dB)
  {
    if (dB > 10.0f) dB = 10.0f;
    return Mathf.Pow(10.0f, dB / 20.0f);
  }

  private void CheckResult(FMOD.RESULT result)
  {
    if (result != FMOD.RESULT.OK)
    {
      if (Debug.isDebugBuild)
      {
        Debug.LogWarning("FMOD warning: result is " + result);
      }
    }
  }
}
