using System.Collections;
using System.Collections.Generic;
using Outclaw;
using UnityEngine;
using Utility;
using Zenject;

namespace Outclaw {
  public interface ISenseManager {
    bool IsSensing { get; }
    bool IsSenseDown { get; }
    bool IsSenseUp { get; }
  }
  
  public class SenseManager : MonoBehaviour, ISenseManager {
    [SerializeField] private AnimationWrapper animationWrapper;
    [Inject] private IPlayerInput playerInput;
    [Inject] private ISenseVisuals senseVisuals;
    
    private float animationProgress;
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
      
      isSenseDown = true;
      isSensing = true;
      animationWrapper.StartNewAnimation(senseVisuals.ActivateSense());
    }

    private bool CancelSense() {
      return playerInput.IsLeft() || playerInput.IsRight() || playerInput.IsJump() || playerInput.IsDownPress() ||
             playerInput.IsInteractDown() || playerInput.IsSneakDown();
    }
    
    private void UpdateSenseUp() {
      if (!isSensing || !playerInput.IsSenseUp() && !CancelSense()) {
        isSenseUp = false;
        return;
      }
      isSenseUp = true;
      isSensing = false;
      animationWrapper.StartNewAnimation(senseVisuals.DeactivateSense());
    }
  }
  /*
  public class SenseManager : MonoBehaviour, ISenseManager {
    [SerializeField] private float senseDelay;
    [SerializeField] private string effectName = "_EffectAmount";
    [SerializeField] private AnimationWrapper animationWrapper;
    
    [Inject] private IPlayerInput playerInput;

    private List<SpriteRenderer> spritesToGrey = new List<SpriteRenderer>();
    private List<ObjectiveInteractable> interactables = new List<ObjectiveInteractable>();
    private HashSet<ObjectiveInteractable> currentInteractablesToGrey;
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

    private bool CancelSense() {
      return playerInput.IsLeft() || playerInput.IsRight() || playerInput.IsJump() || playerInput.IsDownPress() ||
             playerInput.IsInteractDown() || playerInput.IsSneakDown();
    }
    
    private void UpdateSenseUp() {
      if (isSensing && (playerInput.IsSenseUp() || CancelSense())) {
        isSenseUp = true;
        isSensing = false;
        animationWrapper.StartNewAnimation(DeactivateSense());
        return;
      }
      isSenseUp = false;
    }

    public void RegisterSpriteToGrey(SpriteRenderer spriteRenderer) {
      spritesToGrey.Add(spriteRenderer);
    }

    public void RegisterCityInteractable(ObjectiveInteractable objectiveInteractable) {
      interactables.Add(objectiveInteractable);
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
      currentInteractablesToGrey = new HashSet<ObjectiveInteractable>();
      foreach (var interactable in interactables) {
        if (interactable.HasInteraction()) {
          interactable.EnableEffect();
          continue;
        } 
        currentInteractablesToGrey.Add(interactable);
      }
    }
    
    private IEnumerator UpdateElementEffects(float startEffectAmount, float changeEffectAmount) {
      for (var i = 0f; i < senseDelay; i += GlobalConstants.ANIMATION_FREQ) {
        var effectAmount = startEffectAmount + i / senseDelay * changeEffectAmount;
        UpdateElements(effectAmount);
        yield return new WaitForSeconds(GlobalConstants.ANIMATION_FREQ);
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
  }*/
}