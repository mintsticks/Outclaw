using Outclaw;
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

    public void SetOpacity(float opacity) {
      lineRenderer.startColor = lineRenderer.startColor.WithAlpha(opacity);
      lineRenderer.endColor = lineRenderer.endColor.WithAlpha(opacity);
    }

    public void SetColor(Color color) {
      lineRenderer.startColor = color;
      lineRenderer.endColor = color;
    }
  }
}