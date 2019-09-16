using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Outclaw.Heist {
  public enum AbilityType {
    NONE,
    AMBUSH,
    VENT,
    SENSE
  }
  
  [Serializable]
  public class AbilityInfo {
    public AbilityType type;
    public int cooldown;
  }

  public interface IAbilityCooldownManager {
    bool CanUseAbility(AbilityType type);
    void UseAbility(AbilityType type);
  }
  
  public class AbilityCooldownManager : MonoBehaviour, IAbilityCooldownManager {
    [SerializeField]
    private List<AbilityInfo> abilityInfos;

    private Dictionary<AbilityType, bool> abilityStates;

    public void Awake() {
      InitializeAbilityStates();
    }

    public void UseAbility(AbilityType type) {
      abilityStates[type] = false;
      StartCoroutine(CooldownForAbility(type));
    }

    public bool CanUseAbility(AbilityType type) {
      return abilityStates[type];
    }

    private IEnumerator CooldownForAbility(AbilityType type) {
      var info = GetAbilityByType(type);
      yield return new WaitForSeconds(info.cooldown);
      abilityStates[type] = true;
    }

    private AbilityInfo GetAbilityByType(AbilityType type) {
      return abilityInfos.FirstOrDefault(ability => ability.type == type);
    }

    private void InitializeAbilityStates() {
      abilityStates = new Dictionary<AbilityType, bool>();
      foreach (var abilityInfo in abilityInfos) {
        abilityStates.Add(abilityInfo.type, true);
      }
    }
  }
}