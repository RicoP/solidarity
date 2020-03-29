using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
  private void OnTriggerEnter(Collider other)
  {
    CheckGameObject(other.gameObject);
  }

  private void OnCollisionEnter(Collision collision)
  {
    CheckGameObject(collision.gameObject);
  }

  private void CheckGameObject(GameObject gameObject)
  {
    var pickup = gameObject.GetComponent<Pickup>();

    if (pickup) {
      GameDirector.Instance.Collect(pickup);
    }
  }
}
