using UnityEngine;

namespace Outclaw {
  public static class VectorExt {
    public static Vector3 AddToXY(this Vector3 vec, float x, float y) {
      return new Vector3(x + vec.x, y + vec.y, vec.z);
    }
  }
}