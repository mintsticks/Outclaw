﻿using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public interface IPlayer {
    Transform PlayerTransform { get; }
  }
  
  public class Player : MonoBehaviour, IPlayer {
    [SerializeField]
    private MovementController movementController;

    [SerializeField]
    private InteractionController interactionController;
    
    [Inject]
    private IPlayerData playerData;

    public Transform PlayerTransform {
      get { return transform; }
    }

    void FixedUpdate() {
      movementController.UpdateHorizontal();
      movementController.UpdateVertical();
      interactionController.UpdateInteraction();
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
      interactionController.HandleEnter(other);
    }
    
    private void OnTriggerExit2D(Collider2D other) {
      interactionController.HandleExit(other);
    }
  }
}