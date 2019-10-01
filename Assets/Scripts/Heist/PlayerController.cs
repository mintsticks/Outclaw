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
    
    // hiding player
    [SerializeField]
    private GameObject sprite;
    private bool hidden;

    public Transform PlayerTransform {
      get { return transform; }
    }

    void FixedUpdate() {
      if(!hidden){
        movementController.UpdateHorizontal();
        movementController.UpdateVertical();
      }
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