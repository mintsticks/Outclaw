using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public class InteractionController : MonoBehaviour {
    [SerializeField]
    private LayerMask interactableLayer;
    
    [Inject]
    private IPlayerInput playerInput;

    private Interactable currentInteractable;
    
    public void UpdateInteraction() {
      if (playerInput.IsInteract() && currentInteractable != null) {
        currentInteractable.Interact();
      }
    }

    public void HandleEnter(Collider2D other) {
      if ((1 << other.gameObject.layer & interactableLayer) == 0) {
        return;
      }
      
      currentInteractable = other.GetComponent<Interactable>();
      currentInteractable.InRange();
    }

    public void HandleExit(Collider2D other) {
      if ((1 << other.gameObject.layer & interactableLayer) == 0) {
        return;
      }
      
      other.GetComponent<Interactable>().ExitRange();
      currentInteractable = null;
    }
  }
}