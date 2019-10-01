using Outclaw.City;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public class PlayerController : MonoBehaviour, IPlayer {
    [SerializeField]
    private HeistMovementController movementController;
    
    [SerializeField]
    private InteractionController interactionController;

    [SerializeField]
    private AudioClip senseSfx;
    
    [Inject]
    private IAbilityCooldownManager abilityCooldownManager;
    
    [Inject]
    private IPlayerInput playerInput;

    [Inject]
    private ISoundManager soundManager;
    
    public Transform PlayerTransform => transform;
    public Vector3 PlayerVelocity { get; }

    void FixedUpdate() {
      movementController.UpdateMovement();
      interactionController.UpdateInteraction();
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
      interactionController.HandleEnter(other);
    }
    
    private void OnTriggerExit2D(Collider2D other) {
      interactionController.HandleExit(other);
    }

    // Update is called once per frame
    void Update() {
      if (!playerInput.IsSense()) {
        return;
      }

      if (!abilityCooldownManager.CanUseAbility(AbilityType.SENSE)) {
        return;
      }
      
      abilityCooldownManager.UseAbility(AbilityType.SENSE);
      soundManager.PlaySFX(senseSfx);
    }
  }
}