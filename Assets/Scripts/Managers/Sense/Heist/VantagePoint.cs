using UnityEngine;

namespace Outclaw.Heist {
  public class VantagePoint : MonoBehaviour {
    [SerializeField] private Task promptTask;
    [SerializeField] private Indicator indicator;
    
    public Vector3 cameraPosition;
    public float cameraSize;

    public Vector3 CameraPosition => cameraPosition;
    public float HalfCameraHeight => cameraSize;

    public void ShowIndicator() {
      indicator.FadeIn();
    }

    public void UseVantage() {
      if (promptTask == null || promptTask.IsComplete) {
        return;
      }
      promptTask.Complete();
    }
    
    public void HideIndicator() {
      indicator.FadeOut();
    }
  }
}