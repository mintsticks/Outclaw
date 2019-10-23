using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public class DarkenOnSenseElement : MonoBehaviour, ISenseElement {
    [Inject] private ISenseVisuals senseVisuals;
    
    public SpriteRenderer spriteRenderer;
    public Color regularColor;
    public Color darkenedColor;
    
    private void Awake() {
      senseVisuals.RegisterSenseElement(this);
      spriteRenderer.color = regularColor;
    }

    public void UpdateElement(float animationProgress) {
      var color = Color.Lerp(regularColor, darkenedColor, animationProgress);
      spriteRenderer.color = color;
    }
  }
}