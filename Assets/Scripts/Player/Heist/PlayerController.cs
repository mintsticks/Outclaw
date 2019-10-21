using Outclaw.City;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public class PlayerController : MonoBehaviour, IPlayer, IHideablePlayer {
    [SerializeField]
    private MovementController movementController;
    
    [SerializeField]
    private HeistInteractionController interactionController;

    [SerializeField]
    private AudioClip senseSfx;
    
    [Inject]
    private IAbilityCooldownManager abilityCooldownManager;
    
    [Inject]
    private IPlayerInput playerInput;

    [Inject]
    private ISoundManager soundManager;
    
    private bool inputDisabled;

    // hiding player
    [Header("Hiding Player")]
    [SerializeField]
    private SpriteBundle sprites;
    private bool hidden;

    public Transform PlayerTransform => transform;
    public Vector3 PlayerVelocity { get; }
    public bool InputDisabled {
      get => inputDisabled;
      set => inputDisabled = value;
    }

    void FixedUpdate() {
      if(hidden){
        return;
      }
      movementController.UpdatePhysics();
    }

    void Update() {
      interactionController.UpdateInteraction();
      if (hidden) {
        return;
      }
      movementController.UpdateMovement();
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
      interactionController.HandleEnter(other);
    }
    
    private void OnTriggerExit2D(Collider2D other) {
      interactionController.HandleExit(other);
    }

    private void Hide(){
      sprites.enabled = false;
    }

    private void Unhide(){
      sprites.enabled = true;
    }

    public bool Hidden{
      get => hidden;
      set {
        hidden = value;
        if(hidden){
          Hide();
        }
        else{
          Unhide();
        }
      }
    }
  }
}