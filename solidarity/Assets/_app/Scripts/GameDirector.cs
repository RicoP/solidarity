using System.Linq;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

public class GameDirector : MonoBehaviour
{
  static public GameDirector Instance;

  public Color color = new Color(0.0f, 0.0f, 0.5f, 1.0f);
  public float Size = 10;
  public List<string> itemsToCollect = new List<string>();

  private List<string> itemNames = new List<string>();


  void Start()
  {
    StartCoroutine(routine());

    Instance = this;
  }

  private IEnumerator routine()
  {
    yield return new WaitForSeconds(1);

    var pickups = GameObject.FindObjectsOfType<Pickup>();

    var names = pickups.Select(p => p.EnglishName).ToList();
    names.Sort();
    this.itemNames = names.Distinct().ToList();

    foreach (var name in itemNames)
    {
      Debug.Log(name);
    }

    while (true) {
      var randItem = itemNames[UnityEngine.Random.Range(0, itemNames.Count)];
      itemsToCollect.Add(randItem);
      yield return new WaitForSeconds(4 + CalcWaitTime());
    }
  }

  public void Collect(Pickup pickup)
  {
    if (itemsToCollect.Contains(pickup.EnglishName)) {
      itemsToCollect.Remove(pickup.EnglishName);
    }

    if (pickup.transform.parent && pickup.transform.parent.gameObject.Is<HeroController>()) {
      var character = pickup.transform.parent.gameObject.GetComponent<HeroController>();
      character.TossPickupItem();
      character.touchNotifier.RemovePickup(pickup);
    }

    Destroy(pickup.gameObject);
  }

  public float[] waitingTimes = new float[0];

  int difficulty = 0;

  private float CalcWaitTime()
  {
    var time = waitingTimes[difficulty];

    difficulty++;
    difficulty = Mathf.Clamp(difficulty, 0, waitingTimes.Length - 1);

    return time;
  }

  GUIStyle style = new GUIStyle();

  public void OnGUI()
  {
    int w = Screen.width, h = Screen.height;
    style.fontSize = (int)(h * Size / 100.0f);
    style.normal.textColor = color;

    GUILayout.Label("");
    GUILayout.Label("");
    GUILayout.Label("Items to Collect:");

    foreach (var item in itemsToCollect)
    {
      GUILayout.Label("Collect " + item);
    }
  }
}
