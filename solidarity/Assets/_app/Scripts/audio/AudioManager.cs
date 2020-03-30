using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{

  static AudioManager instance;

  [FMODUnity.EventRef]
  [SerializeField]
  private string musicEvent, ambienceEvent;

  FMOD.Studio.EventInstance musicInstance, ambienceInstance;

  FMOD.Studio.VCA musicVCA, sfxVCA;

  [Header("Audio Volumes")]
  [SerializeField]
  [Range(-80f, 10f)]
  private float musicVolume = 0f;

  [SerializeField]
  [Range(-80f, 10f)]
  private float sfxVolume = 0f;

  void Awake()
  {
    if (instance == null)
    {
      instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else if (instance != this)
      Destroy(gameObject);

 
    StartCoroutine(LoadAudio());
  }

  IEnumerator LoadAudio()
  {
    RuntimeManager.LoadBank("Master", true);
    RuntimeManager.LoadBank("Master.strings", true);

    while (!RuntimeManager.HasBankLoaded("Master") || !RuntimeManager.HasBankLoaded("Master.strings"))
    {
      yield return new WaitForEndOfFrame();
    }

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
    ambienceInstance = RuntimeManager.CreateInstance(ambienceEvent);

  }

  private void StartMusicInstance()
  {
    CheckResult(musicInstance.start());
    CheckResult(ambienceInstance.start());
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
