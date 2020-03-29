using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Pickup : MonoBehaviour
{
  [FMODUnity.EventRef]
  public string pickupSound;

  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }
}
