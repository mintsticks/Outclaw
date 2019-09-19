using Outclaw.City;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public class GuardAmbush : MonoBehaviour, Interactable {
    [SerializeField]
    private AbilityType ambushAbilityType = AbilityType.AMBUSH;
    
    [SerializeField]
    private Indicator ambushIndicator;

    [Inject]
    private IAbilityCooldownManager abilityCooldownManager;

    [Inject]
    private IPlayer player;
    
    public void Awake() {
      ambushIndicator.Initialize(player.PlayerTransform);
    }
    
    public void InRange() {
      abilityCooldownManager.SetInAbilityRange(ambushAbilityType, true);
      ambushIndicator.CreateIndicator();
      StartCoroutine(ambushIndicator.FadeIn());
    }

    public void ExitRange() {
      abilityCooldownManager.SetInAbilityRange(ambushAbilityType, false);
      StartCoroutine(ambushIndicator.FadeOut());
    }

    public void Interact() {
      if (!abilityCooldownManager.CanUseAbility(ambushAbilityType)) {
        return;
      }
      
      abilityCooldownManager.UseAbility(ambushAbilityType);
      abilityCooldownManager.SetInAbilityRange(ambushAbilityType, false);
      ambushIndicator.DestroyIndicator();
      Destroy(transform.root.gameObject);
      //TODO(dwong): add sound
    }
  }
}