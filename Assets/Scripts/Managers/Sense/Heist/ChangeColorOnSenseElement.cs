using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public interface ISenseElement {
    void UpdateElement(float animationProgress);
    void OnActivate();
    void OnDeactivate();
  }
  
  public class ChangeColorOnSenseElement : MonoBehaviour, ISenseElement {
    [Inject] private ISenseVisuals senseVisuals;
    
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color regularColor = new Color(1, 1, 1, 0);
    [SerializeField] private Color showColor = new Color(1, 1, 1, 1);
    
    private void Awake() {
      senseVisuals.RegisterSenseElement(this);
      spriteRenderer.color = regularColor;
    }

    public void UpdateElement(float animationProgress) {
      var color = Color.Lerp(regularColor, showColor, animationProgress);
      spriteRenderer.color = color;
    }

    public void OnActivate() { }
    public void OnDeactivate() { }
  }
}