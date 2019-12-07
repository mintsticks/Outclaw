﻿using City;
using Managers;
using Outclaw.City;
using UI.DismissablePrompts;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist {

  public interface IHeistInteractionController {
    void ClearInteractable();
  }
  public class HeistInteractionController : MonoBehaviour, IHeistInteractionController {
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask objectiveInteractableLayer;
    [SerializeField] private LayerMask eventSequenceLayer;
    [SerializeField] private LayerMask oneWayTriggerLayer;
    [SerializeField] private LayerMask conditionalDisplayLayer;
    [SerializeField] private LayerMask guardAttentionLayer;
    [SerializeField] private LayerMask lineOfSightLayer;
    [SerializeField] private LayerMask lightLayer;
    [SerializeField] private LayerMask vantageLayer;
    [SerializeField] private LayerMask checkpointLayer;
    
    [Inject] private IPlayerInput playerInput;
    [Inject] private IPlayer player;
    [Inject] private IPlayerLitManager playerLitManager;
    [Inject] private IVantagePointManager vantagePointManager;
    
    private Interactable currentInteractable;
    private ObjectiveInteractable currentObjectiveInteractable;
    private AttentionZone currentZone;
    private LineOfSight currentLineOfSight;

    public void ClearInteractable() {
      currentInteractable = null;
    }

    public void UpdateInteraction() {
      if (player.InputDisabled) {
        return;
      }

      if (playerInput.IsInteractDown()) {
        currentInteractable?.Interact();
        currentObjectiveInteractable?.Interact();
      }
    }

    public void HandleEnter(Collider2D other) {
      if ((1 << other.gameObject.layer & interactableLayer) != 0) {
        currentInteractable = other.GetComponentInParent<Interactable>();
        currentInteractable.InRange();
      }
      
      if ((1 << other.gameObject.layer & objectiveInteractableLayer) != 0) {
        currentObjectiveInteractable = other.GetComponentInParent<ObjectiveInteractable>();
        currentObjectiveInteractable.InRange();
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
        currentZone.EnterAttention(gameObject);
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

      if ((1 << other.gameObject.layer & checkpointLayer) != 0) {
        var checkpoint = other.GetComponentInParent<Checkpoint>();
        checkpoint.UpdateLastCheckpoint();
      }
      
      if ((1 << other.gameObject.layer & conditionalDisplayLayer) != 0) {
        var conditionalDisplay = other.GetComponentInParent<ConditionalDisplay>();
        conditionalDisplay.Show();
      }
    }

    public void HandleStay(Collider2D other) {
      if ((1 << other.gameObject.layer & guardAttentionLayer) != 0) {
        currentZone = other.GetComponentInChildren<AttentionZone>();
        currentZone.StayAttention(gameObject);
      }
      
      if ((1 << other.gameObject.layer & lineOfSightLayer) != 0) {
        currentLineOfSight = other.GetComponentInChildren<LineOfSight>();
        currentLineOfSight.EnterAttention();
      }
      
      if ((1 << other.gameObject.layer & lightLayer) != 0) {
        playerLitManager.IsLit = true;
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
      if ((1 << other.gameObject.layer & interactableLayer) != 0) {
        //Assumes you can only intersect one interactable at a time.
        other.GetComponentInParent<Interactable>().ExitRange();
        currentInteractable = null;
      }
      
      if ((1 << other.gameObject.layer & objectiveInteractableLayer) != 0) {
        //Assumes you can only intersect one interactable at a time.
        other.GetComponentInParent<ObjectiveInteractable>().ExitRange();
        currentObjectiveInteractable = null;
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
        currentZone.ExitAttention(gameObject);
      }
      
      if ((1 << other.gameObject.layer & conditionalDisplayLayer) != 0) {
        var conditionalDisplay = other.GetComponentInParent<ConditionalDisplay>();
        conditionalDisplay.Hide();
      }
    }
  }
}