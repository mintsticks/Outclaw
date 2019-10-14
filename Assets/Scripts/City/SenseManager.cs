using System.Collections;
using System.Collections.Generic;
using Outclaw;
using UnityEngine;
using Zenject;

namespace City {
  public interface ISenseManager {
    bool IsSensing { get; }
    void RegisterSpriteToGrey(SpriteRenderer spriteRenderer);
    void RegisterCityInteractable(CityInteractable cityInteractable);
  }
  
  public class SenseManager : MonoBehaviour, ISenseManager {
    [SerializeField]
    private float senseDelay;

    [SerializeField]
    private string effectName = "_EffectAmount";
    
    [Inject] 
    private IPlayerInput playerInput;
    
    private float animationFreq = .02f;
    
    private List<SpriteRenderer> spritesToGrey = new List<SpriteRenderer>();
    private List<CityInteractable> interactables = new List<CityInteractable>();
    private HashSet<CityInteractable> currentInteractablesToGrey;
    private IEnumerator currentSenseAnimation;
    private bool isSensing;
    public bool IsSensing => isSensing;
    
    private void Update() {
      UpdateSenseState();
    }
    
    private void UpdateSenseState() {
      UpdateSenseDown();
      UpdateSenseUp();
    }

    private void UpdateSenseDown() {
      if (!playerInput.IsSenseDown()) {
        return;
      }
      
      isSensing = true;
      StopCurrentAnimation();
      StartNewAnimation(ActivateSense());
    }

    private void UpdateSenseUp() {
      if (!playerInput.IsSenseUp()) {
        return;
      }
      
      isSensing = false;
      StopCurrentAnimation();
      StartNewAnimation(DeactivateSense());
    }
    
    private void StopCurrentAnimation() {
      if (currentSenseAnimation == null) {
        return;
      }
      StopCoroutine(currentSenseAnimation);
    }

    private void StartNewAnimation(IEnumerator animation) {
      currentSenseAnimation = animation;
      StartCoroutine(currentSenseAnimation);
    }
    
    public void RegisterSpriteToGrey(SpriteRenderer spriteRenderer) {
      spritesToGrey.Add(spriteRenderer);
    }

    public void RegisterCityInteractable(CityInteractable cityInteractable) {
      interactables.Add(cityInteractable);
    }

    public IEnumerator ActivateSense() {
      UpdateInteractablesOnActivate();
      var startEffectAmount = spritesToGrey[0].material.GetFloat(effectName);
      var changeEffectAmount = 1 - startEffectAmount;
      yield return UpdateElementEffects(startEffectAmount, changeEffectAmount);
    }

    public IEnumerator DeactivateSense() {
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