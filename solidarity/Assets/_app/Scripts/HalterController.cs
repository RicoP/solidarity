using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalterController : MonoBehaviour
{
  public Camera halterCamera;
  public Collider groundCollider;
  public float speed = 10;

  void Awake()
  {

  }

  void Update()
  {
    Ray ray = new Ray(halterCamera.transform.position, halterCamera.transform.forward);
    RaycastHit hit;

    int groundLayer = LayerMask.GetMask("Ground");

    if (Physics.Raycast(ray, out hit, float.PositiveInfinity, groundLayer)) {
      Debug.DrawRay(hit.point, hit.normal * 10, Color.red);
      var upVector = Vector3.Cross(halterCamera.transform.right, hit.normal);
      Debug.DrawRay(hit.point, upVector * 10, Color.blue);
      //Debug.DrawRay(hit.point, Vector3.Cross(halterCamera.transform.up, hit.normal) * 10, Color.green);
      var rightVector = Vector3.Cross(-halterCamera.transform.forward, hit.normal);
      Debug.DrawRay(hit.point, rightVector * 10, Color.cyan);

      var vx = Input.GetAxis("Horizontal");
      var vy = Input.GetAxis("Vertical");

      Vector3 direction = upVector * vy + rightVector * vx;
      direction *= speed;

      var rigidbody = this.GetComponent<Rigidbody>();
      rigidbody.velocity = direction;
    }
  }
}
