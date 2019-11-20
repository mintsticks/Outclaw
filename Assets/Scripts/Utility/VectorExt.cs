using System;
using UnityEngine;

namespace Outclaw {
  public static class VectorExt {
    public static Vector3 AddToXY(this Vector3 vec, float x, float y) {
      return new Vector3(x + vec.x, y + vec.y, vec.z);
    }

    public static bool IsZero(this Vector3 vec) {
      return Mathf.Abs(vec.x) < .0001 && Mathf.Abs(vec.y) < .0001;
    }

    public static void DrawCrosshair(this Vector3 point, Color color){
      Debug.DrawLine(point + (Vector3.up / 3), point + (Vector3.down / 3), color);
      Debug.DrawLine(point + (Vector3.left / 3), point + (Vector3.right / 3), color);
    }
  }
}