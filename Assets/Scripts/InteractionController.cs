using Outclaw.City;
using UnityEngine;
using Zenject;

namespace Outclaw {
  public class InteractionController : MonoBehaviour {
    [SerializeField]
    private LayerMask interactableLayer;

    [SerializeField]
    private LayerMask eventSequenceLayer;
    
    [Inject]
    private IPlayerInput playerInput;

    [Inject] 
    private IPauseMenuManager pauseMenuManager;

    private Interactable currentInteractable;
    
    public void UpdateInteraction() {
      if (pauseMenuManager.IsPaused) {
        return;
      }
      if (playerInput.IsInteractDown()) {
        currentInteractable?.Interact();
      }
    }

    public void HandleEnter(Collider2D other) {
      if ((1 << other.gameObject.layer & interactableLayer) != 0) {
        currentInteractable = other.GetComponentInParent<Interactable>();
        currentInteractable.InRange();
      }

      if ((1 << other.gameObject.layer & eventSequenceLayer) != 0) { 
        var eventSequence = other.GetComponentInParent<EventSequence>();
        StartCoroutine(eventSequence.ExecuteSequence());
      }
    }

    public void HandleExit(Collider2D other) {
      if ((1 << other.gameObject.layer & interactableLayer) == 0) {
        return;
      }
      
      other.GetComponentInParent<Interactable>().ExitRange();
      currentInteractable = null;
    }
  }
}