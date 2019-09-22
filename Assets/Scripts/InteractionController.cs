using UnityEngine;
using Zenject;

namespace Outclaw {
  public class InteractionController : MonoBehaviour {
    [SerializeField]
    private LayerMask interactableLayer;
    
    [Inject]
    private IPlayerInput playerInput;

    private Interactable currentInteractable;
    
    public void UpdateInteraction() {
      if (playerInput.IsInteractDown()) {
        currentInteractable?.Interact();
      }
    }

    public void HandleEnter(Collider2D other) {
      if ((1 << other.gameObject.layer & interactableLayer) == 0) {
        return;
      }

      currentInteractable = other.GetComponentInParent<Interactable>();
      currentInteractable.InRange();
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