using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Outclaw.City {
  public class PromptAnimationComponent : MonoBehaviour {
    [SerializeField]
    private CanvasGroup canvas;

    [SerializeField]
    private AnimationCurve animation;

    [SerializeField]
    private Text promptText;
    
    [SerializeField]
    private float animationTime;

    [SerializeField]
    private float fadeInTime = .5f;

    public void SetText(string text) {
      promptText.text = text;
    }
    
    public IEnumerator AnimatePrompt() {
      for (var i = 0f; i <= fadeInTime; i += Time.deltaTime) {
        canvas.alpha = i / fadeInTime;
        yield return null;
      }
      while (true) {
        for (var i = 0f; i <= animationTime; i += Time.deltaTime) {
          canvas.alpha = animation.Evaluate(i / animationTime);
          yield return null;
        }
      }
    }

    public IEnumerator FadeOut() {
      var startAlpha = canvas.alpha;
      for (var i = fadeInTime; i >= 0; i -= Time.deltaTime) {
        canvas.alpha = startAlpha * i / fadeInTime;
        yield return null;
      }
    }
  }
}