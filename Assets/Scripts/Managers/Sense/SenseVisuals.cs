using System;
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
      foreach (var senseElement in senseElements) {
        senseElement.OnActivate();
      }
      
      vantagePointManager.ToVantageView();
      UpdateFootprints(true);
      yield return UpdateElementEffects(animationProgress, 1 - animationProgress);
    }
    
    public IEnumerator DeactivateSense() {
      foreach (var senseElement in senseElements) {
        senseElement.OnDeactivate();
      }
      
      vantagePointManager.ExitVantageView();
      UpdateFootprints(false);
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

      animationProgress = Mathf.Round(animationProgress);
      UpdateElements();
      UpdateFadeFootprints(false);
    }

    private void UpdateFadeFootprints(bool isFading) {
      foreach (var element in footprints) {
        element.IsFading = isFading;
      }
    }
    
    private void UpdateFootprints(bool enabled) {
      foreach (var element in footprints) {
        element.gameObject.SetActive(enabled);
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

    private float animationProgress;
    
    public IEnumerator ActivateSense() {
      foreach (var senseElement in senseElements) {
        senseElement.OnActivate();
      }

      yield return UpdateElementEffects(animationProgress, 1 - animationProgress);
    }
    
    public IEnumerator DeactivateSense() {
      foreach (var senseElement in senseElements) {
        senseElement.OnDeactivate();
      }
      yield return UpdateElementEffects(animationProgress, -animationProgress);
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
      animationProgress = Mathf.Round(animationProgress);
      UpdateElements();
    }
    
    private void UpdateElements() {
      foreach (var element in senseElements) {
        element.UpdateElement(animationProgress);
      }
    }
  }
}
