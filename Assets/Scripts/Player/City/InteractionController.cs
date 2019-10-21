﻿using City;
using Outclaw.City;
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
    
    [Inject]
    private IPlayerInput playerInput;

    [Inject] 
    private IPlayer player;

    private CityInteractable currentInteractable;
    
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
        currentInteractable = other.GetComponentInParent<CityInteractable>();
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
      
      other.GetComponentInParent<CityInteractable>().ExitRange();
      currentInteractable = null;
    }
  }
}