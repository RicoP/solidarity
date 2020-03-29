using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchNotifier : MonoBehaviour
{
  public List<Ladder> ladders = new List<Ladder>();
  public List<Pickup> pickups = new List<Pickup>();

  public void OnTriggerEnter(Collider other)
  {
    var ladder = other.gameObject.GetComponent<Ladder>();
    if (ladder && !ladders.Contains(ladder))
    {
      ladders.Add(ladder);
    }

    var pickup = other.gameObject.GetComponent<Pickup>();
    if (pickup && !pickups.Contains(pickup))
    {
      pickups.Add(pickup);
    }
  }

  public void OnTriggerExit(Collider other)
  {
    var ladder = other.gameObject.GetComponent<Ladder>();
    if (ladder && ladders.Contains(ladder))
    {
      ladders.Remove(ladder);
    }

    var pickup = other.gameObject.GetComponent<Pickup>();
    if (pickup && pickups.Contains(pickup))
    {
      pickups.Remove(pickup);
    }
  }
}
