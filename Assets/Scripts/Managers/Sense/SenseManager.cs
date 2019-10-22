using System.Collections;
using System.Collections.Generic;
using Outclaw;
using UnityEngine;
using Zenject;

namespace City {
  public interface ISenseManager {
    bool IsSensing { get; }
    bool IsSenseDown { get; }
    bool IsSenseUp { get; }
    void RegisterSpriteToGrey(SpriteRenderer spriteRenderer);
    void RegisterCityInteractable(CityInteractable cityInteractable);
  }
  
  public class SenseManager : MonoBehaviour, ISenseManager {
    [SerializeField] private float senseDelay;
    [SerializeField] private string effectName = "_EffectAmount";
    [SerializeField] private AnimationWrapper animationWrapper;
    
    [Inject] private IPlayerInput playerInput;
    
    private float animationFreq = .02f;

    private List<SpriteRenderer> spritesToGrey = new List<SpriteRenderer>();
    private List<CityInteractable> interactables = new List<CityInteractable>();
    private HashSet<CityInteractable> currentInteractablesToGrey;
    private IEnumerator currentSenseAnimation;
    private bool isSensing;
    private bool isSenseDown;
    private bool isSenseUp;
    
    public bool IsSensing => isSensing;
    public bool IsSenseDown => isSenseDown;
    public bool IsSenseUp => isSenseUp;
    
    private void Update() {
      UpdateSenseState();
    }
    
    private void UpdateSenseState() {
      UpdateSenseDown();
      UpdateSenseUp();
    }

    private void UpdateSenseDown() {
      if (!playerInput.IsSenseDown()) {
        isSenseDown = false;
        return;
      }
      
      isSensing = true;
      isSenseDown = true;
      animationWrapper.StartNewAnimation(ActivateSense());
    }

    private void UpdateSenseUp() {
      if (!playerInput.IsSenseUp()) {
        isSenseUp = false;
        return;
      }

      isSenseUp = true;
      isSensing = false;
      animationWrapper.StartNewAnimation(DeactivateSense());
    }

    public void RegisterSpriteToGrey(SpriteRenderer spriteRenderer) {
      spritesToGrey.Add(spriteRenderer);
    }

    public void RegisterCityInteractable(CityInteractable cityInteractable) {
      interactables.Add(cityInteractable);
    }

    private IEnumerator ActivateSense() {
      UpdateInteractablesOnActivate();
      var startEffectAmount = spritesToGrey[0].material.GetFloat(effectName);
      var changeEffectAmount = 1 - startEffectAmount;
      yield return UpdateElementEffects(startEffectAmount, changeEffectAmount);
    }

    private IEnumerator DeactivateSense() {
      foreach (var interactable in interactables) {
        interactable.DisableEffect();
      }
      var startEffectAmount = spritesToGrey[0].material.GetFloat(effectName);
      yield return UpdateElementEffects(startEffectAmount, -startEffectAmount);
      currentInteractablesToGrey.Clear();
    }
    
    private void UpdateInteractablesOnActivate() {
      currentInteractablesToGrey = new HashSet<CityInteractable>();
      foreach (var interactable in interactables) {
        if (interactable.HasInteraction()) {
          interactable.EnableEffect();
          continue;
        } 
        currentInteractablesToGrey.Add(interactable);
      }
    }
    
    private IEnumerator UpdateElementEffects(float startEffectAmount, float changeEffectAmount) {
      for (var i = 0f; i < senseDelay; i += animationFreq) {
        var effectAmount = startEffectAmount + i / senseDelay * changeEffectAmount;
        UpdateElements(effectAmount);
        yield return new WaitForSeconds(animationFreq);
      }
    }

    private void UpdateElements(float effectAmount) {
      foreach (var sprite in spritesToGrey) {
        sprite.material.SetFloat(effectName, effectAmount);
      }
      foreach (var interactable in currentInteractablesToGrey) {
        interactable.GetSpriteRenderer().material.SetFloat(effectName, effectAmount);
      }
    }
  }
}