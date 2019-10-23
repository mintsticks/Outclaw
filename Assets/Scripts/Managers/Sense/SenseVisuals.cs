using System.Collections;
using System.Collections.Generic;
using Outclaw.City;
using Outclaw.Heist;
using UnityEngine;
using Utility;
using Zenject;

namespace Outclaw {
  public interface ISenseVisuals {
    IEnumerator ActivateSense();
    IEnumerator DeactivateSense();
    void RegisterSenseElement(ISenseElement senseElement);
  }
  
  public class HeistSenseVisuals : ISenseVisuals {
    [Inject] private IVantagePointManager vantagePointManager;

    private List<ISenseElement> senseElements = new List<ISenseElement>();
    private List<Footprint> footprints = new List<Footprint>();
    private float animationProgress;
    
    public IEnumerator ActivateSense() {
      vantagePointManager.ToVantageView();
      yield return UpdateElementEffects(animationProgress, 1 - animationProgress);
    }
    
    public IEnumerator DeactivateSense() {
      vantagePointManager.ExitVantageView();
      yield return UpdateElementEffects(animationProgress, -animationProgress);
    }

    public void RegisterSenseElement(ISenseElement senseElement) {
      if (senseElement is Footprint footprint) {
        footprints.Add(footprint);
      }
      senseElements.Add(senseElement);
    }

    private IEnumerator UpdateElementEffects(float startEffectAmount, float changeEffectAmount) {
      UpdateFadeFootprints(true);
      for (var i = 0f; i < GlobalConstants.SENSE_DELAY; i += GlobalConstants.ANIMATION_FREQ) {
        animationProgress = startEffectAmount + i / GlobalConstants.SENSE_DELAY * changeEffectAmount;
        UpdateElements();
        yield return new WaitForSeconds(GlobalConstants.ANIMATION_FREQ);
      }
      UpdateFadeFootprints(false);
    }

    private void UpdateFadeFootprints(bool isFading) {
      foreach (var element in footprints) {
        element.IsFading = isFading;
      }
    }

    private void UpdateElements() {
      foreach (var element in senseElements) {
        element.UpdateElement(animationProgress);
      }
    }
  }

  public class CitySenseVisuals : ISenseVisuals {
    private List<ISenseElement> senseElements = new List<ISenseElement>();
    private List<ObjectiveInteractable> interactables = new List<ObjectiveInteractable>();
    private HashSet<ObjectiveInteractable> currentInteractablesToGrey;
    
    private float animationProgress;
    
    public IEnumerator ActivateSense() {
      UpdateInteractablesOnActivate();
      yield return UpdateElementEffects(animationProgress, 1 - animationProgress);
    }
    
    public IEnumerator DeactivateSense() {
      foreach (var interactable in interactables) {
        interactable.DisableEffect();
      }
      yield return UpdateElementEffects(animationProgress, -animationProgress);
      currentInteractablesToGrey.Clear();
    }

    public void RegisterSenseElement(ISenseElement senseElement) {
      if (senseElement is ObjectiveInteractable interactable) {
        interactables.Add(interactable);
      }
      senseElements.Add(senseElement);
    }

    private IEnumerator UpdateElementEffects(float startEffectAmount, float changeEffectAmount) {
      for (var i = 0f; i < GlobalConstants.SENSE_DELAY; i += GlobalConstants.ANIMATION_FREQ) {
        animationProgress = startEffectAmount + i / GlobalConstants.SENSE_DELAY * changeEffectAmount;
        UpdateElements();
        yield return new WaitForSeconds(GlobalConstants.ANIMATION_FREQ);
      }
    }
    
    private void UpdateElements() {
      foreach (var element in senseElements) {
        element.UpdateElement(animationProgress);
      }
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
  }
}
