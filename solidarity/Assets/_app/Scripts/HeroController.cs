using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeroController : MonoBehaviour
{
  public enum Kind { 
    None,
    Holder,
    Packer
  }

  public Kind kind = Kind.None;

  public Camera halterCamera;
  public TouchNotifier touchNotifier;

  public float speed = 10;

  private int groundLayer;
  private Rigidbody rigidbody;
  private GameObject holdingObject = null;
  private Ladder snappedToLadder;


  private string AxisHorz = "halter_horz";
  private string AxisVert = "halter_vert";
  private string BtnAction = "halter_action";

  void Awake()
  {
    groundLayer = LayerMask.GetMask("Ground");
    rigidbody = this.GetComponent<Rigidbody>();

    switch (kind)
    {
      case Kind.Holder:
        AxisHorz = "halter_horz";
        AxisVert = "halter_vert";
        BtnAction = "halter_action";
        break;

      case Kind.Packer:
        AxisHorz = "packer_horz";
        AxisVert = "packer_vert";
        BtnAction = "packer_action";
        break;

      case Kind.None:
      default:
        throw new ArgumentOutOfRangeException();
    }
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

      var vx = Input.GetAxis(AxisHorz);
      var vy = Input.GetAxis(AxisVert);
      var action = Input.GetButtonDown(BtnAction);
      if (action)
      {
        Debug.Log("action!");
        ExecuteAction();
      }

      Vector3 direction = upVector * vy + rightVector * vx;
      direction *= speed;

      rigidbody.velocity = direction;
    }
  }

  private void ExecuteAction()
  {
    if (kind == Kind.Holder)
    {
      HolderAction();
    }
    if (kind == Kind.Packer)
    {
      PackerAction();
    }

  }

  private void PackerAction()
  {
    if (snappedToLadder == null && touchNotifier.ladders.Any())
    {
      SnapToLadder(touchNotifier.ladders.First());
    }
    else
    {
      //UnsnapFromLadder();
    }
  }

  private void SnapToLadder(Ladder ladder)
  {
    Debug.Log("SnapToLadder");

    //snappedToLadder = ladder;
  }

  private void HolderAction()
  {
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


    ladder.transform.up = Vector3.up;
    var pos = ladder.transform.position;
    pos.y = ladder.LadderLength / 2;
    ladder.transform.position = pos;

    ladder.transform.parent = this.transform;
  }

  private static void MakeTouchable(Ladder ladder, bool touchable)
  {
    var laddercollider = ladder.GetComponent<Collider>();
    laddercollider.isTrigger = !touchable;

    var ladderbody = ladder.GetComponent<Rigidbody>();
    ladderbody.isKinematic = !touchable;
  }
}
