using System;
using UnityEngine;

namespace Outclaw {
  public static class LineHelperExt {
    public static bool FindLineIntersection(Vector2 o0, Vector2 d0, Vector2 o1, Vector2 d1, ref Vector2 i) {
      var s0 = d0 - o0;
      var s1 = d1 - o1;
      var det = -s1.x * s0.y + s0.x * s1.y;
      
      if (Math.Abs(det) < .0001) {
        return false;
      }
      
      var s = (-s0.y * (d0.x - o1.x) + s0.x * (d0.y - o1.y)) / det;
      var t = (s1.x * (o0.y - o1.y) - s1.y * (o0.x - o1.x)) / det;

      if (!(s >= 0) || !(s <= 1) || !(t >= 0) || !(t <= 1)) {
        return false;
      }
      
      i = new Vector2(o0.x + t * s0.x, o0.y + t * s0.y);
      return true;
    }
  }
}