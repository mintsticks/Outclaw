using UnityEngine;

namespace UI.Dialogue {
  public class BubbleTail : MonoBehaviour {
    [SerializeField] private LineRenderer lineRenderer;
    
    public void UpdatePoints(Vector3 origin, Vector3 destination) {
      for (var i = 0; i < lineRenderer.positionCount; i++) {
        var position = Vector3.Lerp(origin, destination, (float)i / lineRenderer.positionCount);
        lineRenderer.SetPosition(i, position);
      }
    }
  }
}