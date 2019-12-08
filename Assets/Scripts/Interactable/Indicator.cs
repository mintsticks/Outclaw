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
    [SerializeField] private InputType inputType = InputType.INTERACT;
    [SerializeField] private GameObject tutorialParent;
    [SerializeField] private Image tutorialImage;
    [SerializeField] private Text tutorialText;
    [SerializeField] private PlatformInfo info;
    [SerializeField] private Task indicatorTask;
    [SerializeField] private float disabledAlpha = .5f;

    [SerializeField] private CanvasGroup canvasGroup;
    
    private float animationProgress;
    private bool showKey = true;
    private InteractableState state;
    
    private void Start() {
      tutorialImage.sprite = info.images.FirstOrDefault(i => i.platform == Application.platform)?.image;
      tutorialText.text = InputStringHelper.GetStringForInput(inputType);
    }

    public void FadeIn() {
      if(state != InteractableState.Enabled){
        animationWrapper.StartNewAnimation(FadeInAnim());
        state = InteractableState.Enabled;
      }
      if (showKey) {
        return;
      }
      UpdateTutorial();
    }

    private void UpdateTutorial() {
      tutorialParent.SetActive(!indicatorTask.IsComplete);
    }
    
    public void FadeOut() {
      if(state != InteractableState.Invisible){
        animationWrapper.StartNewAnimation(FadeOutAnim());
        state = InteractableState.Invisible;
      }
    }

    public void FadeToDisabled(){
      if(state != InteractableState.DisabledVisible){
        animationWrapper.StartNewAnimation(FadeToDisabledAnim());
        state = InteractableState.DisabledVisible;
      }
    }

    private IEnumerator FadeInAnim() {
      image.enabled = true;
      yield return UpdateIndicator(animationProgress, 1);
    }

    private IEnumerator FadeOutAnim() {
      yield return UpdateIndicator(animationProgress, 0);
      image.enabled = false;
    }

    private IEnumerator FadeToDisabledAnim() {
      image.enabled = true;
      yield return UpdateIndicator(animationProgress, disabledAlpha);
    }
    
    private IEnumerator UpdateIndicator(float startAmount, float endAmount) {
      float changeAmount = endAmount - startAmount;
      for (var i = 0f; i < fadeTime; i += GlobalConstants.ANIMATION_FREQ) {
        animationProgress = startAmount + i / fadeTime * changeAmount;
        UpdateIndicator();
        yield return new WaitForSeconds(GlobalConstants.ANIMATION_FREQ);
      }
      
      animationProgress = endAmount;
      UpdateIndicator();
    }
    
    private void UpdateIndicator() {
      canvasGroup.alpha = animationProgress;
    }
  }
}