using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomListener : MonoBehaviour
{
  [SerializeField]
  private Transform player, cam;

  // Spieler 1/links =  0
  // Spieler 2/rechts = 1
  [SerializeField]
  private int listener;

  FMOD.ATTRIBUTES_3D attributes = new FMOD.ATTRIBUTES_3D();

  FMOD.VECTOR playerPosition;

  void Start()
  {
    FMODUnity.RuntimeManager.StudioSystem.setNumListeners(2);
    player = this.transform;
  }

  void Update()
  {
    attributes.forward = FMODUnity.RuntimeUtils.ToFMODVector(cam.forward);
    attributes.up = FMODUnity.RuntimeUtils.ToFMODVector(cam.up);
    attributes.position = FMODUnity.RuntimeUtils.ToFMODVector(cam.position);
    playerPosition = FMODUnity.RuntimeUtils.ToFMODVector(player.position);

    FMODUnity.RuntimeManager.StudioSystem.setListenerAttributes(listener, attributes, playerPosition);
  }
}
