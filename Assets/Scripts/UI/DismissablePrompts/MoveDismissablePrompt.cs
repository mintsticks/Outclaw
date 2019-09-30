using System.Collections;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public class MoveDismissablePrompt : MonoBehaviour, IDismissablePrompt {
    [SerializeField]
    private PromptAnimationComponent promptAnimationComponent;
    
    [Inject]
    private IPlayerInput playerInput;

    private bool movedLeft;
    private bool movedRight;
    private IEnumerator animationCoroutine;

    private void Awake() {
      animationCoroutine = promptAnimationComponent.AnimatePrompt();
      StartCoroutine(animationCoroutine);
    }

    private void Update() {
      if (playerInput.IsLeft()) {
        movedLeft = true;
      }
      if (playerInput.IsRight()) {
        movedRight = true;
      }
    }
    
    public bool IsDismissable() {
      return movedLeft && movedRight;
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