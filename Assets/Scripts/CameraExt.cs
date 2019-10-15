using UnityEngine;

namespace Outclaw {
  public static class CameraExt {
    public static Bounds OrthographicBounds(this Camera camera) {
      var screenAspect = Screen.width / (float)Screen.height;
      var cameraHeight = camera.orthographicSize * 2;
      return new Bounds(
        camera.transform.position,
        new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
    }
  }
}