using System;
using UnityEngine;

namespace Outclaw {
  public static class VectorExt {
    public static Vector3 AddToXY(this Vector3 vec, float x, float y) {
      return new Vector3(x + vec.x, y + vec.y, vec.z);
    }

    public static Vector3 WithX(this Vector3 vec, float x) {
      return new Vector3(x, vec.y, vec.z);
    }
    
    public static bool IsZero(this Vector3 vec) {
      return Mathf.Abs(vec.x) < .0001 && Mathf.Abs(vec.y) < .0001;
    }
  }
}