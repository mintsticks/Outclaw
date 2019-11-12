using System;
using UnityEngine;

namespace Utility {
  public static class VectorUtil {
    /// <summary>
    /// Finds a point in the circumference for a circle with a given radius and center.
    /// Points are specified by their angle on the unit circle.
    /// </summary>
    public static Vector3 GetPositionForAngle(Vector3 origin, float radius, float angle) {
      var rad = Mathf.Deg2Rad * angle;
      return origin + radius * new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
    }
  }
}