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
    private AudioSource abilitySound = null;

    [Inject]
    private IPlayerInput playerInput;

    public Transform PlayerTransform {
      get { return transform; }
    }

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
      if (playerInput.IsSense()) {
        SenseAbility ability = this.gameObject.GetComponent<SenseAbility>();
        ability.UseAbility();
        if (ability.Useable) {
          abilitySound.Play();
        }
      }
    }
  }
}