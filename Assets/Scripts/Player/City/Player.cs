using System;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public interface IPlayer {
    Transform PlayerTransform { get; }
    Transform HeadTransform { get; }
    bool InputDisabled { get; set; }
    void UpdatePosition(Vector3? position);
    Vector3 Velocity { get; }
  }

  public class Player : MonoBehaviour, IPlayer {
    [SerializeField] private MovementController movementController;
    [SerializeField] private InteractionController interactionController;
    [SerializeField] private Transform headTransform;
    
    [Inject] private IPlayerData playerData;
    [Inject] private IPauseGame pauseGame;
    [Inject] private ISpawnManager spawnManager;

    private bool inputDisabled;

    public Transform PlayerTransform => transform;
    public Transform HeadTransform => headTransform;
    public Vector3 Velocity => movementController.Velocity;
    public bool InputDisabled {
      get => inputDisabled || pauseGame.IsPaused;
      set => inputDisabled = value;
    }

    private void Start() {
      Vector3? spawnPoint = spawnManager.GetSpawnPoint();
      if (spawnPoint == null) {
        return;
      }

      transform.position = spawnPoint.Value;
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

    public void UpdatePosition(Vector3? spawnPoint) {
      if (spawnPoint == null) {
        return;
      }

      transform.position = spawnPoint.Value;
    }
  }
}