using System.Collections;
using Outclaw;
using Outclaw.City;
using UnityEngine;
using Zenject;

namespace UI.DismissablePrompts {
  public class SneakDismissablePrompt : MonoBehaviour {
    [SerializeField]
    private PromptAnimationComponent promptAnimationComponent;
    
    [Inject]
    private IPlayerInput playerInput;

    private bool sneaked;
    private IEnumerator animationCoroutine;

    private void Awake() {
      animationCoroutine = promptAnimationComponent.AnimatePrompt();
      StartCoroutine(animationCoroutine);
    }

    private void Update() {
      if (playerInput.IsSneakDown()) {
        sneaked = true;
      }
    }
    
    public bool IsDismissable() {
      return sneaked;
    }

    public IEnumerator DismissPrompt() {
      //TODO: add some effect when completed
      StopCoroutine(animationCoroutine);
      yield return promptAnimationComponent.FadeOut();
      Destroy(gameObject);
    }
    
    public void SetPromptText(string text) {
      promptAnimationComponent.SetText(text);
    }
  }
}