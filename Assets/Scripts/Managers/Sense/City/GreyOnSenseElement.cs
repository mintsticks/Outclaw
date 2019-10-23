using Outclaw.Heist;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public class GreyOnSenseElement : MonoBehaviour, ISenseElement {
    [SerializeField] private string effectName = "_EffectAmount";
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    [Inject] private ISenseVisuals senseVisuals;
    
    private void Awake() {
      senseVisuals.RegisterSenseElement(this);
    }

    public void UpdateElement(float animationProgress) {
      spriteRenderer.material.SetFloat(effectName, animationProgress);
    }
  }
}