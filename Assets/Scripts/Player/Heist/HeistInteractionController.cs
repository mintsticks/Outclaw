using City;
using Managers;
using Outclaw.City;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public class HeistInteractionController : MonoBehaviour {
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask eventSequenceLayer;
    [SerializeField] private LayerMask oneWayTriggerLayer;
    [SerializeField] private LayerMask guardAttentionLayer;
    [SerializeField] private LayerMask lineOfSightLayer;
    [SerializeField] private LayerMask lightLayer;
    [SerializeField] private LayerMask vantageLayer;
    
    [Inject] private IPlayerInput playerInput;
    [Inject] private IPlayer player;
    [Inject] private IPlayerLitManager playerLitManager;
    [Inject] private IVantagePointManager vantagePointManager;
    
    private Interactable currentInteractable;
    private AttentionZone currentZone;
    private LineOfSight currentLineOfSight;

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
      
      if ((1 << other.gameObject.layer & lineOfSightLayer) != 0) {
        currentLineOfSight = other.GetComponentInChildren<LineOfSight>();
        currentLineOfSight.EnterAttention();
      }
      
      if ((1 << other.gameObject.layer & vantageLayer) != 0) {
        var vantage = other.GetComponentInChildren<VantagePoint>();
        vantage.ShowIndicator();
        vantagePointManager.RegisterCurrentVantage(vantage);
      }
      
      if ((1 << other.gameObject.layer & lightLayer) != 0) {
        playerLitManager.IsLit = true;
      }
    }

    public void HandleStay(Collider2D other) {
      if ((1 << other.gameObject.layer & guardAttentionLayer) != 0) {
        currentZone = other.GetComponentInChildren<AttentionZone>();
        currentZone.StayAttention();
      }
      
      if ((1 << other.gameObject.layer & lineOfSightLayer) != 0) {
        currentLineOfSight = other.GetComponentInChildren<LineOfSight>();
        currentLineOfSight.EnterAttention();
      }
      
      if ((1 << other.gameObject.layer & lightLayer) != 0) {
        playerLitManager.IsLit = true;
      }
    }

    public void HandleExit(Collider2D other) {
      if ((1 << other.gameObject.layer & interactableLayer) != 0) {
        //Assumes you can only intersect one interactable at a time.
        other.GetComponentInParent<Interactable>().ExitRange();
        currentInteractable = null;
      }

      if ((1 << other.gameObject.layer & vantageLayer) != 0) {
        vantagePointManager.ResetVantage();
      }
      
      if ((1 << other.gameObject.layer & guardAttentionLayer) != 0) {
        currentZone = null;
      }
      
      if ((1 << other.gameObject.layer & lightLayer) != 0) {
        playerLitManager.IsLit = false;
      }

      if ((1 << other.gameObject.layer & guardAttentionLayer) != 0) {
        currentZone = other.GetComponentInChildren<AttentionZone>();
        currentZone.ExitAttention();
      }
    }
  }
}