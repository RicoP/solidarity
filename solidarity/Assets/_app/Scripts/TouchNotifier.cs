using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchNotifier : MonoBehaviour
{
  public List<Ladder> ladders = new List<Ladder>();

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public void OnTriggerEnter(Collider other)
  {
    var ladder = other.gameObject.GetComponent<Ladder>();
    if (ladder && !ladders.Contains(ladder)) {
      ladders.Add(ladder);
    }
  }

  public void OnTriggerExit(Collider other)
  {
    var ladder = other.gameObject.GetComponent<Ladder>();
    if (ladder && ladders.Contains(ladder))
    {
      ladders.Remove(ladder);
    }
  }
}
