using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Outclaw {
  public class Indicator : MonoBehaviour {
    [SerializeField] private float fadeTime = .25f;
    [SerializeField] private AnimationWrapper animationWrapper;
    [SerializeField] private Image image;

    [SerializeField] private GameObject worldParent;
    [SerializeField] private GameObject indicatorParent;
    [SerializeField] private GameObject tutorialParent;
    [SerializeField] private Image tutorialImage;
    [SerializeField] private Text tutorialText;
    [SerializeField] private PlatformInfo info;
    [SerializeField] private Task indicatorTask;

    [SerializeField] private CanvasGroup canvasGroup;
    
    private float animationProgress;
    private Camera main;
    private bool active;
    
    private void Start() {
      if (indicatorTask.IsComplete) {
        return;
      }
      Platform currentPlatform;
#if UNITY_WSA
      currentPlatform = Platform.XBOX;
#else
      currentPlatform = Platform.PC;
#endif
      tutorialImage.sprite = info.images.FirstOrDefault(i => i.platform == currentPlatform)?.image;
      tutorialText.text = info.texts.FirstOrDefault(i => i.platform == currentPlatform)?.text;
      main = Camera.main;
    }

    public void Update() {
      if (!active) {
        return;
      }
      indicatorParent.transform.position = main.WorldToScreenPoint(worldParent.transform.position);
    }
    
    public void FadeIn() {
      UpdateTutorial();
      animationWrapper.StartNewAnimation(FadeInAnim());
    }

    private void UpdateTutorial() {
      tutorialParent.SetActive(!indicatorTask.IsComplete);
    }
    
    public void FadeOut() {
      animationWrapper.StartNewAnimation(FadeOutAnim());
    }

    private IEnumerator FadeInAnim() {
      image.enabled = true;
      active = true;
      yield return UpdateIndicator(animationProgress, 1 - animationProgress);
    }

    private IEnumerator FadeOutAnim() {
      yield return UpdateIndicator(animationProgress,-animationProgress);
      image.enabled = false;
      active = false;
    }
    
    private IEnumerator UpdateIndicator(float startAmount, float changeAmount) {
      for (var i = 0f; i < fadeTime; i += GlobalConstants.ANIMATION_FREQ) {
        animationProgress = startAmount + i / fadeTime * changeAmount;
        UpdateIndicator();
        yield return new WaitForSeconds(GlobalConstants.ANIMATION_FREQ);
      }
      
      animationProgress = Mathf.Round(animationProgress);
      UpdateIndicator();
    }
    
    private void UpdateIndicator() {
      canvasGroup.alpha = animationProgress;
    }
  }
}