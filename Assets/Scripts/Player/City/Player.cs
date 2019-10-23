using System;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public interface IPlayer {
    Transform PlayerTransform { get; }
    Vector3 PlayerVelocity { get; }
    bool InputDisabled { get; set; }
  }
  
  public class Player : MonoBehaviour, IPlayer {
    [SerializeField]
    private MovementController movementController;

    [SerializeField]
    private InteractionController interactionController;
    
    [Inject]
    private IPlayerData playerData;

    [Inject] 
    private IPauseGame pauseGame;

    [Inject] 
    private ISpawnManager spawnManager;
    
    private bool inputDisabled;
    
    public Transform PlayerTransform => transform;

    public Vector3 PlayerVelocity => movementController.Velocity;

    public bool InputDisabled {
      get => inputDisabled || pauseGame.IsPaused;
      set => inputDisabled = value;
    }

    private void Start() {
      Vector3 spawnPoint = spawnManager.GetSpawnPoint();
      Vector3 falsePosition = new Vector3(-1,-1,-1);
      if (spawnPoint == falsePosition) {
        return;
      }
      transform.position = spawnPoint;
    }

    void FixedUpdate() {
      movementController.UpdatePhysics();
    }

    void Update() {
      movementController.UpdateMovement();
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