using System;
using Managers;
using Outclaw.City;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public class PlayerController : MonoBehaviour, IPlayer, IHideablePlayer {
    [SerializeField] private HeistMovementController movementController;
    [SerializeField] private HeistInteractionController interactionController;
    [SerializeField] private AudioClip senseSfx;
    [SerializeField] private SpriteController spriteController;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform headTransform;
    [SerializeField] private BoxCollider2D visualBounds;
    
    [Inject] private IAbilityCooldownManager abilityCooldownManager;
    [Inject] private IPlayerInput playerInput;
    [Inject] private ISoundManager soundManager;
    [Inject] private IPauseGame pauseGame;
    [Inject] private ISpawnManager spawnManager;

    private bool inputDisabled;

    // hiding player
    [Header("Hiding Player")] [SerializeField]
    private SpriteBundle sprites;

    private bool hidden;

    public Transform PlayerTransform => transform;
    public Bounds PlayerBounds => visualBounds.bounds;
    public Transform HeadTransform => headTransform;
    public Vector3 Velocity => movementController.Velocity;
    
    public bool InputDisabled {
      get => inputDisabled || pauseGame.IsPaused;
      set => inputDisabled = value;
    }

    void FixedUpdate() {
      if (hidden) {
        return;
      }

      movementController.UpdatePhysics();
    }

    void Update() {
      interactionController.UpdateInteraction();
      spriteController.UpdateColor();
      if (hidden) {
        return;
      }

      movementController.UpdateMovement();
    }

    private void OnTriggerEnter2D(Collider2D other) {
      interactionController.HandleEnter(other);
    }

    private void OnTriggerStay2D(Collider2D other) {
      interactionController.HandleStay(other);
    }

    private void OnTriggerExit2D(Collider2D other) {
      interactionController.HandleExit(other);
    }

    private void Hide() {
      sprites.enabled = false;
    }

    private void Unhide() {
      sprites.enabled = true;
    }

    public bool Hidden {
      get => hidden;
      set {
        hidden = value;
        if (hidden) {
          Hide();
          return;
        }

        Unhide();
      }
    }
    
    public void UpdatePosition(Vector3? spawnPoint) {
      if (spawnPoint == null) {
        return;
      }
      transform.position = spawnPoint.Value;
      rb.position = spawnPoint.Value;
    }
  }
}