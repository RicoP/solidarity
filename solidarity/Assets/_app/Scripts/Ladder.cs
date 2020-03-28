using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
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

  public Vector3 PivotLokal
  {
    get
    {
      return -Up * (LadderLength * 0.5f);
    }
  }

  public Vector3 Pivot
  {
    get
    {
      return transform.position + PivotLokal;
    }
  }

  public float LadderLength
  {
    get
    {
      //TODO: make better
      var s = this.transform.localScale;
      return Mathf.Max(s.x, s.y, s.z);
    }
  }


  void Update()
  {
    upVectorRaw = Vector3.Project(Vector3.up, transform.up);
    upVectorNormalized = upVectorRaw.normalized;

    Debug.Log(upVectorRaw);

    Debug.DrawRay(transform.position, Up * 10, Color.red);
  }

  private void OnDrawGizmos()
  {
    Gizmos.DrawSphere(Pivot, 1);
  }
}
