using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leiter : MonoBehaviour
{
  private Vector3 upVectorRaw;
  private Vector3 upVectorNormalized;

  public bool Standing
  {
    get
    {
      return upVectorRaw.magnitude > 0.2f;
    }
  }

  public Vector3 Up
  {
    get
    {
      if (!Standing)
      {
        return Vector3.zero;
      }
      return upVectorNormalized;
    }
  }


  void Start()
  {
  }

  void Update()
  {
    upVectorRaw = Vector3.Project(Vector3.up, transform.up);
    upVectorNormalized = upVectorRaw.normalized;

    Debug.Log(upVectorRaw);

    Debug.DrawRay(transform.position, Up * 10, Color.red);
  }
}
