using Outclaw.City;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public class PlayerController : MonoBehaviour, IPlayer, IHideablePlayer {
    [SerializeField]
    private MovementController movementController;
    
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

    // hiding player
    [SerializeField]
    private GameObject sprite;
    private bool hidden;

    void FixedUpdate() {
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
      sprite.SetActive(false);
    }

    private void Unhide(){
      sprite.SetActive(true);
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