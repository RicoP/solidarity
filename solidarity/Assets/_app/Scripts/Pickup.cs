using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Pickup : MonoBehaviour
{
  [FMODUnity.EventRef]
  public string pickupSound;

  public string EnglishName = "UNTITLED";

  public Texture2D icon;
}
