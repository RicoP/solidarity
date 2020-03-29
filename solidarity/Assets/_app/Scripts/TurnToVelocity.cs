using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnToVelocity : MonoBehaviour
{
  Rigidbody rigidbody;

  void Start()
  {
    this.rigidbody = transform.parent.GetComponent<Rigidbody>();
  }

  void Update()
  {
    var velocity = rigidbody.velocity;
    velocity.y = 0; //project to xz plane

    if (velocity.sqrMagnitude > 0)
    {
      transform.forward = velocity.normalized;
    }
  }
}
