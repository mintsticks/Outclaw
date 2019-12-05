using System.Collections;
using UnityEngine;

namespace UI.Dialogue {
  public class BubbleAnimationHelper : MonoBehaviour {
    [SerializeField] private float bubbleFadeTime;
    [SerializeField] private AnimationCurve bubbleFade;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private BubbleTail bubbleTail;
    
    public IEnumerator FadeBubble() {
      if (bubbleTail != null) {
        bubbleTail.SetOpacity(0);
      }
      
      for (var t = 0f; t <= bubbleFadeTime; t += Time.deltaTime) {
        canvasGroup.alpha = 1 - bubbleFade.Evaluate(t / bubbleFadeTime);
        yield return null;
      }
    }
  }
}