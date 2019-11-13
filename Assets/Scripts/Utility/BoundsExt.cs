using UnityEngine;

namespace Outclaw {
  public static class BoundsExt {
    public static Bounds ScreenToWorld(this Bounds bounds, Camera cam) {
      var bound = new Bounds();
      bound.min = cam.ScreenToWorldPoint(bounds.min);
      bound.max = cam.ScreenToWorldPoint(bounds.max);
      return bound;
    }

    public static float AreaOverlap(this Bounds bounds, Bounds toOverlap) {
      var overlapWidth = Mathf.Min(bounds.max.x, toOverlap.max.x) - Mathf.Max(bounds.min.x, toOverlap.min.x);
      var overlapHeight = Mathf.Min(bounds.max.y, toOverlap.max.y) - Mathf.Max(bounds.min.y, toOverlap.min.y);
      var area = overlapHeight * overlapWidth;
      return Mathf.Clamp(area, 0f, area);
    }
    
    public static bool IsFullyInBounds(this Bounds bubbleBound, Bounds bounds) {
      return bounds.Contains(bubbleBound.min) && bounds.Contains(bubbleBound.max);
    }
  }
}