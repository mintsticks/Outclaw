using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public class JumpDismissablePrompt : MonoBehaviour, IDismissablePrompt {
    [SerializeField]
    private PromptAnimationComponent promptAnimationComponent;
    
    [Inject]
    private IPlayerInput playerInput;

    private bool jumped;
    private IEnumerator animationCoroutine;

    private void Awake() {
      animationCoroutine = promptAnimationComponent.AnimatePrompt();
      StartCoroutine(animationCoroutine);
    }

    private void Update() {
      if (playerInput.IsJump()) {
        jumped = true;
      }
    }
    
    public bool IsDismissable() {
      return jumped;
    }

    public void DismissPrompt() {
      //TODO: add some effect when completed
      StopCoroutine(animationCoroutine);
      Destroy(gameObject);
    }
    
    public void SetPromptText(string text) {
      promptAnimationComponent.SetText(text);
    }
  }
}