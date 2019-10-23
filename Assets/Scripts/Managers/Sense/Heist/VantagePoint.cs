using UnityEngine;

namespace Outclaw.Heist {
  public class VantagePoint : MonoBehaviour {
    public Vector3 cameraPosition;
    public float cameraSize;

    [SerializeField] private Indicator indicator;
    
    public void ShowIndicator() {
      indicator.FadeIn();
    }
    
    public void HideIndicator() {
      indicator.FadeOut();
    }
  }
}