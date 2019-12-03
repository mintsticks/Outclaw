using System;
using UnityEngine;

namespace Utility {
  public static class FloatExt {
    public static bool IsZero(this float num) {
      return Math.Abs(num) < GlobalConstants.TOLERANCE;
    }
    
    public static float ClampedAdd(this float num, float toAdd, float min, float max) {
      return Mathf.Clamp(num + toAdd, min, max);
    }
  }
}