using System.Collections;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public class InteractDismissablePrompt : MonoBehaviour, IDismissablePrompt {
    [SerializeField]
    private PromptAnimationComponent promptAnimationComponent;

    [Inject]
    private IDialogueManager dialogueManager;

    [Inject]
    private IPlayerInput playerInput;
    
    private IEnumerator animationCoroutine;

    private void Awake() {
      animationCoroutine = promptAnimationComponent.AnimatePrompt();
      StartCoroutine(animationCoroutine);
    }

    public bool IsDismissable() {
      //TODO(dwong): check if player is in range of an interactable with dialogue instead.
      return dialogueManager.IsDialogueRunning && playerInput.IsInteract();
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