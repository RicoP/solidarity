using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
  static public bool Is<T>(this GameObject that) where T : Component
  {
    return that.GetComponent<T>() != null;
  }
}
