using System;
using System.Collections;
using Antlr4.Runtime.Atn;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using Zenject;

namespace Outclaw {
  public class PromptDisplay : MonoBehaviour {
    public class Factory : PlaceholderFactory<Data, PromptDisplay> { }

    public class Data {
      public PromptData Info;
    }

    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private Text title;
    [SerializeField] private Text description;
    [SerializeField] private Text dismissText;
    [SerializeField] private InputType dismissInput;
    [SerializeField] private Image image;

    [SerializeField] private AnimationWrapper animationWrapper;
    [SerializeField] private float fadeInTime = .1f;
    [Inject] private IPlayerInput playerInput;

    private bool isDismissed;
    private bool isFadingOut;

    public bool IsDismissed => isDismissed;
    
    [Inject]
    public void Initialize(Data data) {
      HandleTitleData(data);
      HandleDescriptionData(data);
      HandleImageData(data);
    }

    private void Awake() {
      animationWrapper.StartNewAnimation(FadeInContent());
    }

    private void Update() {
      CheckDismiss();
    }

    private void HandleTitleData(Data data) {
      if (!data.Info.hasTitle) {
        return;
      }

      title.text = data.Info.promptTitle;
    }
    
    private void HandleImageData(Data data) {
      if (!data.Info.hasImage) {
        image.gameObject.SetActive(false);
        return;
      }

      image.sprite = data.Info.promptImage;
    }

    private void HandleDescriptionData(Data data) {
      dismissText.text = "- PRESS " + InputStringHelper.GetStringForInput(dismissInput) + " TO CONTINUE -";
      if (PlatformUtil.GetPlatform() == Platform.XBOX_ONE) {
        description.text = ParseDescription(data.Info.xboxDescription);
        return;
      }
      
      description.text = ParseDescription(data.Info.defaultPromptDescription);
    }

    private string ParseDescription(string text) {
      return text.Replace("<n>", "\n");
    }
    
    
    private void CheckDismiss() {
      if (!playerInput.IsInteractDown() || isFadingOut) {
        return;
      }
      animationWrapper.StartNewAnimation(FadeOutContent());
    }
    
    private IEnumerator FadeInContent() {
      for (var i = 0f; i < fadeInTime; i += GlobalConstants.ANIMATION_FREQ) {
        canvas.alpha = i / fadeInTime;
        yield return new WaitForSecondsRealtime(GlobalConstants.ANIMATION_FREQ);
      }

      canvas.alpha = 1;
    }
    
    private IEnumerator FadeOutContent() {
      isFadingOut = true;
      for (var i = fadeInTime; i >= 0; i -= GlobalConstants.ANIMATION_FREQ) {
        canvas.alpha = i / fadeInTime;
        yield return new WaitForSecondsRealtime(GlobalConstants.ANIMATION_FREQ);
      }

      canvas.alpha = 0;
      isFadingOut = false;
      isDismissed = true;
    }
  }
}