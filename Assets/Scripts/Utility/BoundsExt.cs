using UnityEngine;

namespace Outclaw {
  public static class BoundsExt {
    public static Bounds ScreenToWorld(this Bounds bounds, Camera cam) {
      var bound = new Bounds();
      bound.min = cam.ScreenToWorldPoint(bounds.min);
      bound.max = cam.ScreenToWorldPoint(bounds.max);
      return bound;
    }
  }
}