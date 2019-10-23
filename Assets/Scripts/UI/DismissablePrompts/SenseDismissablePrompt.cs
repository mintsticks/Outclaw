using System.Collections;
using Outclaw;
using Outclaw.City;
using UnityEngine;
using Zenject;

namespace UI.DismissablePrompts {
  public class SenseDismissablePrompt : MonoBehaviour, IDismissablePrompt {
    [SerializeField]
    private PromptAnimationComponent promptAnimationComponent;
    
    [Inject]
    private IPlayerInput playerInput;

    private bool sensed;
    private IEnumerator animationCoroutine;

    private void Awake() {
      animationCoroutine = promptAnimationComponent.AnimatePrompt();
      StartCoroutine(animationCoroutine);
    }

    private void Update() {
      if (playerInput.IsSenseDown()) {
        sensed = true;
      }
    }
    
    public bool IsDismissable() {
      return sensed;
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