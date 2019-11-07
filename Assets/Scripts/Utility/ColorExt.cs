using UnityEngine;

namespace Outclaw {
  public static class ColorExt {
    public static Color WithAlpha(this Color color, float alpha) {
      return new Color(color.r, color.g, color.b, alpha);
    }
  }
}