using System.Collections;
using City;
using Outclaw.City;
using Player;
using UnityEngine;
using Zenject;

namespace Outclaw {
  public class InteractionController : MonoBehaviour {
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask eventSequenceLayer;
    [SerializeField] private LayerMask oneWayTriggerLayer;
    [SerializeField] private AnimationController animationController;

    [Inject] private IPlayerInput playerInput;
    [Inject] private IPlayer player;

    private ObjectiveInteractable currentInteractable;
    private IHaveTask currentTask;
    private bool queuedInteraction;
    
    public void UpdateInteraction() {
      if (player.InputDisabled || 
          !playerInput.IsInteractDown() || 
          currentInteractable == null) {
        return;
      }
      
      if (!currentInteractable.HasInteraction()) {
        return;
      }

      if (queuedInteraction) {
        return;
      }
      
      if (player.Velocity != Vector3.zero) {
        StartCoroutine(QueueInteraction());
        return;
      }
      Interact();
    }

    private void Interact() {
      var isLeftOfObject = player.PlayerTransform.position.x < currentInteractable.ObjectiveTransform.position.x;
      animationController.TurnCharacter(!isLeftOfObject);
      currentInteractable?.Interact();
    }
    
    private IEnumerator QueueInteraction() {
      queuedInteraction = true;
      while (!player.Velocity.IsZero()) {
        yield return null;
      }
      Interact();
      queuedInteraction = false;
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