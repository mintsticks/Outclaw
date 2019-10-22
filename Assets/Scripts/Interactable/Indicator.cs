using System.Collections;
using UnityEngine;

namespace Outclaw {
  public class Indicator : MonoBehaviour {
    [SerializeField] private float fadeTime = .25f;
    [SerializeField] private AnimationWrapper animationWrapper;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Color minColor = new Color(1, 1, 1, 0);
    [SerializeField] private Color maxColor = new Color(1, 1, 1, 1);
    
    private float animationFreq = .02f;
    private float animationProgress;

    public void FadeIn() {
      Debug.Log(animationWrapper);
      animationWrapper.StartNewAnimation(FadeInAnim());
    }
    
    public void FadeOut() {
      animationWrapper.StartNewAnimation(FadeOutAnim());
    }

    public IEnumerator FadeInAnim() {
      spriteRenderer.enabled = true;
      yield return UpdateIndicator(animationProgress, 1 - animationProgress);
    }
    
    public IEnumerator FadeOutAnim() {
      yield return UpdateIndicator(animationProgress,-animationProgress);
      spriteRenderer.enabled = false;
    }
    
    private IEnumerator UpdateIndicator(float startAmount, float changeAmount) {
      for (var i = 0f; i < fadeTime; i += animationFreq) {
        animationProgress = startAmount + i / fadeTime * changeAmount;
        UpdateIndicator();
        yield return new WaitForSeconds(animationFreq);
      }
    }
    
    private void UpdateIndicator() {
      var color = Color.Lerp(minColor, maxColor, animationProgress);
      spriteRenderer.color = color;
    }
  }
}