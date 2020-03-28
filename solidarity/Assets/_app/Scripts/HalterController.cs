using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HalterController : MonoBehaviour
{
  public Camera halterCamera;
  public TouchNotifier touchNotifier;

  public float speed = 10;

  private int groundLayer;
  private Rigidbody rigidbody;
  private GameObject holdingObject = null;

  void Awake()
  {
    groundLayer = LayerMask.GetMask("Ground");
    rigidbody = this.GetComponent<Rigidbody>();
  }

  void Update()
  {
    Ray ray = new Ray(halterCamera.transform.position, halterCamera.transform.forward);
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit, float.PositiveInfinity, groundLayer))
    {
      Debug.DrawRay(hit.point, hit.normal * 10, Color.red);
      var upVector = Vector3.Cross(halterCamera.transform.right, hit.normal);
      Debug.DrawRay(hit.point, upVector * 10, Color.blue);
      //Debug.DrawRay(hit.point, Vector3.Cross(halterCamera.transform.up, hit.normal) * 10, Color.green);
      var rightVector = Vector3.Cross(-halterCamera.transform.forward, hit.normal);
      Debug.DrawRay(hit.point, rightVector * 10, Color.cyan);

      var vx = Input.GetAxis("halter_horz");
      var vy = Input.GetAxis("halter_vert");
      var action = Input.GetButtonDown("halter_action");
      if (action)
      {
        Debug.Log("action!");
        if (holdingObject)
        {
          if (holdingObject.Is<Ladder>())
          {
            TossLadder(holdingObject.GetComponent<Ladder>());
          }
        }
        else
        {
          if (touchNotifier.ladders.Any())
          {
            PickUpLadder(touchNotifier.ladders.First());
          }
        }
      }

      Vector3 direction = upVector * vy + rightVector * vx;
      direction *= speed;

      rigidbody.velocity = direction;
    }
  }

  private void TossLadder(Ladder ladder)
  {
    ladder.transform.parent = null;
    MakeTouchable(ladder, true);
    holdingObject = null;
  }

  private void PickUpLadder(Ladder ladder)
  {
    holdingObject = ladder.gameObject;

    MakeTouchable(ladder, false);

    ladder.transform.parent = this.transform;

    ladder.transform.localPosition = Vector3.up * ladder.LadderLength / 2;
  }

  private static void MakeTouchable(Ladder ladder, bool touchable)
  {
    var laddercollider = ladder.GetComponent<Collider>();
    laddercollider.isTrigger = !touchable;

    var ladderbody = ladder.GetComponent<Rigidbody>();
    ladderbody.isKinematic = !touchable;
  }
}
