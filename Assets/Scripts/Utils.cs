using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
  public static void DestroyChildren(Transform transform)
  {
    int childCount = transform.childCount;
    for (int i = childCount - 1; i >= 0; i--)
    {
      Transform child = transform.GetChild(i);
      Object.Destroy(child.gameObject);
      // Or use DestroyImmediate(child.gameObject) for immediate destruction
    }
    transform.DetachChildren();
  }
}
