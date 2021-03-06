﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeroController : MonoBehaviour
{
  public enum Kind
  {
    None,
    Holder,
    Packer
  }

  public Kind kind = Kind.None;

  public Camera halterCamera;
  public TouchNotifier touchNotifier;

  public float speed = 10;
  private float ladderClimpSpeed = 4;

  private int groundLayer;
  protected Rigidbody rigidbody;
  private HeroController otherhero;
  private charControl charController1;
  private scndcharControl charController2;
  private GameObject holdingObject = null;
  private Ladder snappedToLadder;


  private string AxisHorz = "halter_horz";
  private string AxisVert = "halter_vert";
  private string BtnAction = "halter_action";
  private Pickup pickup;

  float timer = 0.0f;
  [SerializeField]
  float footstepSpeed = 0.3f;

  void Awake()
  {
    groundLayer = LayerMask.GetMask("Ground");
    rigidbody = this.GetComponent<Rigidbody>();
    charController1 = this.GetComponentInChildren<charControl>();
    charController2 = this.GetComponentInChildren<scndcharControl>();

    var heros = GameObject.FindObjectsOfType<HeroController>();
    otherhero = (heros[0] == this) ? heros[1] : heros[0];

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
      var dirx = Input.GetAxis(AxisHorz);
      var diry = Input.GetAxis(AxisVert);
      var action = Input.GetButtonDown(BtnAction);

      Debug.DrawRay(hit.point, hit.normal * 2, Color.blue);
      var upVector = Vector3.Cross(halterCamera.transform.right, hit.normal);
      Debug.DrawRay(hit.point, upVector * 2, Color.green);
      var rightVector = Vector3.Cross(-halterCamera.transform.forward, hit.normal);
      Debug.DrawRay(hit.point, rightVector * 2, Color.red);

      if (action)
      {
        Debug.Log("action!");
        ExecuteAction();
      }

      if (snappedToLadder == null)
      {
        Vector3 direction = upVector * diry + rightVector * dirx;
        direction *= speed;
        direction.y = rigidbody.velocity.y; //keep gravity

        rigidbody.velocity = direction;

        SetBool("walking", rigidbody.velocity.magnitude > 0.1f);

        if (Mathf.Abs(rigidbody.velocity.x) > 0.5f && kind == Kind.Holder)
        {
          PlayFootstepSound();
        }
      
      }
      else
      {
        if (touchNotifier.ladders.Contains(snappedToLadder))
        {
          if (snappedToLadder.Standing)
          {
            Vector3 direction = snappedToLadder.Up * diry;
            direction *= ladderClimpSpeed;
            rigidbody.velocity = direction + snappedToLadder.GetComponent<Rigidbody>().velocity;
            if (otherhero.holdingObject && otherhero.holdingObject.Is<Ladder>()) {
              rigidbody.velocity += otherhero.rigidbody.velocity;
            }

            if (Mathf.Abs(rigidbody.velocity.y) > 0.1f)
            {
              PlayFootstepLeiterSound();
            }

            Debug.Log(transform.position.y);
            if (transform.position.y <= 2.1 && diry < 0)
            {
              UnsnapFromLadder();
            }
          }
          else
          {
            UnsnapFromLadder();
          }
        }
        else
        {
          UnsnapFromLadder();
        }
      }
    }
  }

  private void SetBool(string anim, bool on)
  {
    if (charController1 != null) charController1.anim.SetBool(anim, on);
    if (charController2 != null) charController2.anim.SetBool(anim, on);
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

    if (pickup != null)
    {
      TossPickupItem();
    }
    else if (touchNotifier.pickups.Any())
    {
      //sort pickup items by distance and take first
      touchNotifier.pickups.Sort((lhs, rhs) => {
        var lhsDistance = (lhs.transform.position - this.transform.position).sqrMagnitude;
        var rhsDistance = (rhs.transform.position - this.transform.position).sqrMagnitude;
        return (int)Mathf.Sign(lhsDistance - rhsDistance);
      });
      PickUpPickupItem(touchNotifier.pickups.First());
    }

  }

  public void TossPickupItem()
  {
    pickup.GetComponent<Rigidbody>().isKinematic = false;
    pickup.GetComponent<Collider>().isTrigger = false;
    pickup.transform.parent = null;
    pickup = null;
  }

  private void PickUpPickupItem(Pickup item)
  {
    pickup = item;
    pickup.GetComponent<Rigidbody>().isKinematic = true;
    pickup.GetComponent<Collider>().isTrigger = true;
    pickup.transform.parent = this.transform;
    pickup.transform.localPosition = new Vector3(0, 2, 0);

    PlayPickUpSound(pickup);
  }

  private void UnsnapFromLadder()
  {
    SetBool("climb_ladder", false);

    snappedToLadder.Occupied = false;
    snappedToLadder = null;
    transform.parent = null;
  }

  private void SnapToLadder(Ladder ladder)
  {
    Debug.Log("SnapToLadder");

    SetBool("climb_ladder", true);

    ladder.Occupied = true;
    snappedToLadder = ladder;
    transform.parent = ladder.transform;
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
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Environment/Pickup/LeiterPickup", this.gameObject);
      }
    }
  }

  private void TossLadder(Ladder ladder)
  {
    SetBool("pickup", false);

    ladder.transform.parent = null;
    MakeTouchable(ladder, true);
    holdingObject = null;
  }

  private void PickUpLadder(Ladder ladder)
  {
    if (ladder.Occupied) return;

    SetBool("pickup", true);

    holdingObject = ladder.gameObject;

    MakeTouchable(ladder, false);

    ladder.transform.parent = this.transform;

    ladder.transform.up = Vector3.up;
    var pos = Vector3.zero;
    pos.y = ladder.LadderLength / 2;
    pos.z = -1;
    ladder.transform.localPosition = pos;
  }

  private static void MakeTouchable(Ladder ladder, bool touchable)
  {
    var laddercollider = ladder.GetComponent<Collider>();
    laddercollider.isTrigger = !touchable;

    var ladderbody = ladder.GetComponent<Rigidbody>();
    ladderbody.isKinematic = !touchable;
  }


  #region SFX

  private void PlayPickUpSound(Pickup pickup)
  {
    if (FMODUnity.RuntimeManager.StudioSystem.getEvent(pickup.pickupSound, out _) == FMOD.RESULT.OK)
    {
      FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Player/Pickup", this.gameObject);
      FMODUnity.RuntimeManager.PlayOneShotAttached(pickup.pickupSound, pickup.gameObject);
    }
    else
    {
      FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Player/Pickup", this.gameObject);
    }
  }

  private void PlayFootstepSound()
  {
    if (timer > footstepSpeed)
    {
      string sound = "event:/Player/Footsteps Player 1";
      FMODUnity.RuntimeManager.PlayOneShotAttached(sound, this.gameObject);
      timer = 0.0f;
    }

    timer += Time.deltaTime;
  }

  private void PlayFootstepLeiterSound()
  {
    if (timer > footstepSpeed)
    {
      string sound = "event:/Player/Footsteps Leiter";
      FMODUnity.RuntimeManager.PlayOneShotAttached(sound, this.gameObject);
      timer = 0.0f;
    }

    timer += Time.deltaTime;
  }

  #endregion
}
