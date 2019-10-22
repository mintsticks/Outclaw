using City;
using Outclaw.City;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public class HeistInteractionController : MonoBehaviour {
    [SerializeField]
    private LayerMask interactableLayer;

    [SerializeField]
    private LayerMask eventSequenceLayer;

    [SerializeField]
    private LayerMask oneWayTriggerLayer;

    [SerializeField] 
    private LayerMask guardAttentionLayer;
    
    [Inject]
    private IPlayerInput playerInput;

    [Inject] 
    private IPauseGame pause;

    [Inject] 
    private IPlayer player;
    
    private Interactable currentInteractable;
    private AttentionZone currentZone;
    
    public void UpdateInteraction() {
      if (player.InputDisabled) {
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
      
      if ((1 << other.gameObject.layer & oneWayTriggerLayer) != 0) { 
        var oneWayPlatform = other.GetComponentInParent<OneWayPlatform>();
        oneWayPlatform.IntersectTrigger();
      }
      
      if ((1 << other.gameObject.layer & guardAttentionLayer) != 0) {
        currentZone = other.GetComponentInChildren<AttentionZone>();
        currentZone.EnterAttention();
      }
    }

    public void HandleStay(Collider2D other) {
      if ((1 << other.gameObject.layer & guardAttentionLayer) != 0) {
        currentZone.EnterAttention();
      }
    }
    
    public void HandleExit(Collider2D other) {
      if ((1 << other.gameObject.layer & interactableLayer) != 0) {
        other.GetComponentInParent<Interactable>().ExitRange();
        currentInteractable = null;
      }
      
      if ((1 << other.gameObject.layer & guardAttentionLayer) != 0) {
        currentZone = null;
      }
    }
  }
}