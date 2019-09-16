using Outclaw.City;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public class Vent : MonoBehaviour, Interactable {
    [SerializeField]
    private AbilityType ventAbilityType = AbilityType.VENT;

    [SerializeField]
    private Vector3 ventOffset;
    
    [SerializeField]
    private Vent destination;

    [SerializeField]
    private Indicator ventIndicator;
    
    [Inject]
    private IAbilityCooldownManager abilityCooldownManager;

    [Inject]
    private IPlayer player;

    public void Awake() {
      ventIndicator.Initialize(player.PlayerTransform);
    }
    
    public void InRange() {
      //TODO(dwong): update UI to show vent enabled
      //ventImage.color = Color.white;
      //ventText.enabled = true;
      
      ventIndicator.CreateIndicator();
      StartCoroutine(ventIndicator.FadeIn());
    }

    public void ExitRange() {
      StartCoroutine(ventIndicator.FadeOut());
    }

    public void Interact() {
      if (!abilityCooldownManager.CanUseAbility(ventAbilityType)) {
        return;
      }
      
      abilityCooldownManager.UseAbility(ventAbilityType);
      
      player.PlayerTransform.position = destination.transform.position + ventOffset;
      
      //TODO(dwong): add sound
    }
  }
}