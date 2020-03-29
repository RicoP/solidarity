using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
  public bool Occupied = false;

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
      var s = this.GetComponent<BoxCollider>().size;
      return Mathf.Max(s.x, s.y, s.z);
    }
  }


  void Update()
  {
    upVectorRaw = Vector3.Project(Vector3.up, transform.up);
    upVectorNormalized = upVectorRaw.normalized;

    if (Occupied)
    {
      Yittering();
    }

    Debug.DrawRay(transform.position, Up * 10, Color.red);
  }

  private void Yittering()
  {
    /*float x = Random.value * 2 - 1;
    float y = Random.value * 2 - 1;
    float z = Random.value * 2 - 1;
    var v = new Vector3(x, y, z);
    v *= .05f;

    this.GetComponent<Rigidbody>().velocity += v;*/
  }

  private void OnDrawGizmos()
  {
    Gizmos.DrawSphere(Pivot, 1);
  }
}
