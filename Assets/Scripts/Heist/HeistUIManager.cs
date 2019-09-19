using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Outclaw.Heist {
  [Serializable]
  public class AbilityIconInfo {
    public AbilityType type;
    public Image icon;
  }
  
  public class HeistUIManager : MonoBehaviour {
    [SerializeField]
    private List<AbilityIconInfo> abilityIcons;

    [SerializeField]
    private Color cooldownColor;
    
    [SerializeField]
    private Color readyColor;
    
    [Inject]
    private IAbilityCooldownManager abilityCooldownManager;

    private void Update() {
      foreach (var ability in abilityIcons) {
        ability.icon.color = !abilityCooldownManager.CanUseAbility(ability.type) ? cooldownColor : readyColor;
      }
    }
  }
}