using System.Collections;
using City;
using Outclaw.City;
using Player;
using UI.DismissablePrompts;
using UnityEngine;
using Zenject;

namespace Outclaw {
  public class InteractionController : MonoBehaviour {
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask eventSequenceLayer;
    [SerializeField] private LayerMask oneWayTriggerLayer;
    [SerializeField] private LayerMask conditionalDisplayLayer;
    [SerializeField] private AnimationController animationController;

    [Inject] private IPlayerInput playerInput;
    [Inject] private IPlayer player;

    private ObjectiveInteractable currentInteractable;
    private IHaveTask currentTask;

    public void UpdateInteraction() {
      var invalidInput = player.InputDisabled || !playerInput.IsInteractDown();
      var invalidInteractable = currentInteractable == null || !currentInteractable.HasInteraction();
      if (invalidInput || invalidInteractable) {
        return;
      }
      
      Interact();
    }

    private void Interact() {
      var isLeftOfObject = player.PlayerTransform.position.x < currentInteractable.ObjectiveTransform.position.x;
      animationController.TurnCharacter(!isLeftOfObject);
      currentInteractable?.Interact();
      //TODO: zero out velocity. make colliders at base of feet so it cannot be interacted while jumping
    }

    private void UpdateCurrentInteractable(Collider2D other) {
      if (!player.IsGrounded && currentInteractable != null) {
        other.GetComponentInParent<ObjectiveInteractable>().ExitRange();
        currentInteractable = null;
        return;
      }

      if (currentInteractable != null || !player.IsGrounded) {
        return;
      }
      
      currentInteractable = other.GetComponentInParent<ObjectiveInteractable>();
      currentInteractable.InRange();
    }
    
    public void HandleEnter(Collider2D other) {
      if ((1 << other.gameObject.layer & eventSequenceLayer) != 0) {
        var eventSequence = other.GetComponentInParent<EventSequence>();
        StartCoroutine(eventSequence.ExecuteSequence());
      }

      if ((1 << other.gameObject.layer & oneWayTriggerLayer) != 0) {
        var oneWayPlatform = other.GetComponentInParent<OneWayPlatform>();
        oneWayPlatform.IntersectTrigger();
      }
      
      if ((1 << other.gameObject.layer & conditionalDisplayLayer) != 0) {
        var conditionalDisplay = other.GetComponentInParent<ConditionalDisplay>();
        conditionalDisplay.Show();
      }
    }

    public void HandleStay(Collider2D other) {
      if ((1 << other.gameObject.layer & interactableLayer) != 0) {
        UpdateCurrentInteractable(other);
      }
      
      if ((1 << other.gameObject.layer & eventSequenceLayer) != 0) {
        var eventSequence = other.GetComponentInParent<EventSequence>();
        StartCoroutine(eventSequence.ExecuteSequence());
      }
      
      if ((1 << other.gameObject.layer & conditionalDisplayLayer) != 0) {
        var conditionalDisplay = other.GetComponentInParent<ConditionalDisplay>();
        conditionalDisplay.UpdateCondition();
      }
    }

    public void HandleExit(Collider2D other) {
      if (currentInteractable != null && (1 << other.gameObject.layer & interactableLayer) != 0) {
        other.GetComponentInParent<ObjectiveInteractable>().ExitRange();
        currentInteractable = null;
      }
      
      if ((1 << other.gameObject.layer & conditionalDisplayLayer) != 0) {
        var conditionalDisplay = other.GetComponentInParent<ConditionalDisplay>();
        conditionalDisplay.Hide();
      }
    }
  }
}