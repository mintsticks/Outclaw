using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist{
  public class ShowCanvasOnSense : MonoBehaviour, ISenseElement {
    
    [SerializeField] private CanvasGroup canvas;

    [Inject] private ISenseVisuals senseVisuals;

    private void Awake() {
      senseVisuals.RegisterSenseElement(this);

      canvas.alpha = 0;
    }

    public void UpdateElement(float animationProgress) {
      canvas.alpha = animationProgress;
    }

    public void OnActivate() { }
    public void OnDeactivate() { }
  }
}
