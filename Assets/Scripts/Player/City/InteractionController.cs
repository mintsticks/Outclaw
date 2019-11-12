using City;
using Outclaw.City;
using Player;
using UnityEngine;
using Zenject;

namespace Outclaw {
  public class InteractionController : MonoBehaviour {
    [SerializeField]
    private LayerMask interactableLayer;

    [SerializeField]
    private LayerMask eventSequenceLayer;

    [SerializeField]
    private LayerMask oneWayTriggerLayer;

    [SerializeField]
    private AnimationController animationController;
    
    [Inject]
    private IPlayerInput playerInput;

    [Inject] 
    private IPlayer player;

    private ObjectiveInteractable currentInteractable;
    private IHaveTask currentTask;
    public void UpdateInteraction() {
      if (player.InputDisabled) {
        return;
      }

      if (!playerInput.IsInteractDown() || currentInteractable == null) {
        return;
      }

      
      if (!currentInteractable.HasInteraction()) {
        return;
      }
      var isLeftOfObject = player.PlayerTransform.position.x < currentInteractable.ObjectiveTransform.position.x;
      animationController.TurnCharacter(!isLeftOfObject);
      currentInteractable?.Interact();
    }

    public void HandleEnter(Collider2D other) {
      if ((1 << other.gameObject.layer & interactableLayer) != 0) {
        currentInteractable = other.GetComponentInParent<ObjectiveInteractable>();
        currentInteractable.InRange();
      }

      if ((1 << other.gameObject.layer & eventSequenceLayer) != 0) { 
        var eventSequence = other.GetComponentInParent<EventSequence>();
        StartCoroutine(eventSequence.ExecuteSequence());
      }
      
      if ((1 << other.gameObject.layer & oneWayTriggerLayer) != 0) { 
        var oneWayPlatform = other.GetComponentInParent<OneWayPlatform>();
        oneWayPlatform.IntersectTrigger();
      }
    }

    public void HandleExit(Collider2D other) {
      if ((1 << other.gameObject.layer & interactableLayer) == 0) {
        return;
      }
      
      other.GetComponentInParent<ObjectiveInteractable>().ExitRange();
      currentInteractable = null;
    }
  }
}