using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public interface IPlayer {
    Transform PlayerTransform { get; }
    Vector3 PlayerVelocity { get; }
  }
  
  public class Player : MonoBehaviour, IPlayer {
    [SerializeField]
    private MovementController movementController;

    [SerializeField]
    private InteractionController interactionController;
    
    [Inject]
    private IPlayerData playerData;

    [Inject]
    private IDialogueManager dialogueManager;
    
    public Transform PlayerTransform => transform;

    public Vector3 PlayerVelocity => movementController.Velocity;

    void FixedUpdate() {
      movementController.UpdateMovement();
    }

    void Update() {
      if (dialogueManager.IsDialogueRunning) {
        return;
      }
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