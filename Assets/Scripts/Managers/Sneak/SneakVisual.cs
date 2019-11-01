using System.Collections;
using Outclaw;
using UnityEngine;
using UnityEngine.UI;

namespace Managers {
  public class SneakVisual : MonoBehaviour {
    [SerializeField] private Image ring;
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private AnimationWrapper animationWrapper;
    [SerializeField] private float fadeTime = .25f;
    [SerializeField] private float pulseTime = .05f;
    [SerializeField] private AnimationCurve pulseAnimationCurve;
    [SerializeField] private float delayFadeIn = .5f;
    public void SetSneakPercent(float percent) {
      ring.fillAmount = percent;
    }

    public void StopAnimation() {
      animationWrapper.StopCurrentAnimation();
    }
    
    public void Pulse() {
      animationWrapper.StartNewAnimation(PulseAnimation());
    }

    public void DelayedFadeOut() {
      var alpha = canvas.alpha;
      animationWrapper.StartNewAnimation(DelayedFadeAnim(delayFadeIn, alpha, -alpha));
    }
    
    public void FadeIn() {
      var alpha = canvas.alpha;
      animationWrapper.StartNewAnimation(FadeAnim(alpha, 1 - alpha));
    }

    public void FadeOut() {
      var alpha = canvas.alpha;
      animationWrapper.StartNewAnimation(FadeAnim(alpha, -alpha));
    }

    public void SetVisible() {
      canvas.alpha = 1;
    }

    private IEnumerator DelayedFadeAnim(float delay, float startAlpha, float toAddAlpha) {
      yield return new WaitForSeconds(delay);
      yield return FadeAnim(startAlpha, toAddAlpha);
    }

    private IEnumerator PulseAnimation() {
      while (true) {
        for (var i = 0f; i <= pulseTime; i += Time.deltaTime) {
          canvas.alpha = pulseAnimationCurve.Evaluate(i / pulseTime);
          yield return null;
        }
      }
    }

    private IEnumerator FadeAnim(float startAlpha, float toAddAlpha) {
      for (var i = 0f; i < fadeTime; i += Time.deltaTime) {
        canvas.alpha = startAlpha + i / fadeTime * toAddAlpha;
        yield return null;
      }

      canvas.alpha = startAlpha + toAddAlpha;
    }
  }
}