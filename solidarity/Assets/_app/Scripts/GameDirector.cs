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
  public List<Pickup> itemsToCollect = new List<Pickup>();

  private List<Pickup> items = new List<Pickup>();


  void Start()
  {
    StartCoroutine(routine());

    Instance = this;
  }

  private IEnumerator routine()
  {
    yield return new WaitForSeconds(1);

    var pickups = GameObject.FindObjectsOfType<Pickup>().ToList();

    items.Clear();

    pickups.Sort((lhs, rhs) => lhs.EnglishName.CompareTo(rhs.EnglishName));

    foreach (var p in pickups)
    {
      if (object.Equals(p.icon, null)) continue;
      if (p.icon == null) continue;
      if (p.EnglishName == "UNTITLED") continue;

      var current = items.LastOrDefault();
      if (current == null)
      {
        items.Add(p);
      }
      else
      {
        if (!current.EnglishName.Equals(p.EnglishName))
        {
          items.Add(p);
        }
      }
    }

    foreach (var i in items)
    {
      Debug.Log(i.EnglishName);
    }

    while (true)
    {
      var randItem = items[UnityEngine.Random.Range(0, items.Count)];
      itemsToCollect.Add(randItem);
      FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Inventar", (float)itemsToCollect.Count);
      yield return new WaitForSeconds(4 + CalcWaitTime());
    }
  }

  public void Collect(Pickup pickup)
  {
    int index = itemsToCollect.FindIndex(p => p.EnglishName == pickup.EnglishName);
    bool found = index != -1;
    if (found)
    {
      itemsToCollect.RemoveAt(index);
    }

    if (pickup.transform.parent && pickup.transform.parent.gameObject.Is<HeroController>())
    {
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
    GUILayout.Label("Items to Collect:");

    foreach (var item in itemsToCollect)
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label(item.icon, GUILayout.MaxHeight(64));
      GUILayout.Label("Collect " + item.EnglishName);
      GUILayout.EndHorizontal();
    }
  }
}
