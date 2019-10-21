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
    
    [SerializeField]
    private AudioClip ventSound;

//    [Inject]
//    private IAbilityCooldownManager abilityCooldownManager;

    [Inject]
    private IPlayer player;

    [Inject]
    private ISoundManager soundManager;

    public void Awake() {
      ventIndicator.Initialize(player.PlayerTransform);
    }
    
    public void InRange() {
//      abilityCooldownManager.SetInAbilityRange(ventAbilityType, true);
      ventIndicator.CreateIndicator();
      StartCoroutine(ventIndicator.FadeIn());
    }

    public void ExitRange() {
//      abilityCooldownManager.SetInAbilityRange(ventAbilityType, false);
      StartCoroutine(ventIndicator.FadeOut());
    }

    public void Interact() {
//      if (!abilityCooldownManager.CanUseAbility(ventAbilityType)) {
//        return;
//      }
//      
//      abilityCooldownManager.UseAbility(ventAbilityType);
      
      player.PlayerTransform.position = destination.transform.position + ventOffset;
      
      //TODO(dwong): add sound
      soundManager.PlaySFX(ventSound);
    }
  }
}