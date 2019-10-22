using System.Collections;
using System.Collections.Generic;
using Outclaw;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public interface IHeistSenseManager {
    void RegisterElementToDarken(DarkenOnSenseElement element);
    void RegisterElementToShow(ShowOnSenseElement element);
    void RegisterFootprint(Footprint element);
  }
  
  public class HeistSenseManager : MonoBehaviour, IHeistSenseManager {
    [SerializeField] private float senseDelay;
    [SerializeField] private AnimationWrapper animationWrapper;
    [Inject] private IPlayerInput playerInput;
    
    private float animationFreq = .02f;
    private List<DarkenOnSenseElement> elementsToDarken = new List<DarkenOnSenseElement>();
    private List<ShowOnSenseElement> elementsToShow = new List<ShowOnSenseElement>();
    private List<Footprint> footprints = new List<Footprint>();
    private float animationProgress;
    
    private bool isSensing;

    public void RegisterElementToDarken(DarkenOnSenseElement element) {
      elementsToDarken.Add(element);
    }

    public void RegisterElementToShow(ShowOnSenseElement element) {
      elementsToShow.Add(element);
    }

    public void RegisterFootprint(Footprint element) {
      footprints.Add(element);
    }
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
      animationWrapper.StartNewAnimation(ActivateSense());
    }

    private bool CancelSense() {
      return playerInput.IsLeft() || playerInput.IsRight() || playerInput.IsJump() || playerInput.IsDownPress() ||
             playerInput.IsInteractDown() || playerInput.IsSneakDown();
    }
    
    private void UpdateSenseUp() {
      if (!isSensing || !playerInput.IsSenseUp() && !CancelSense()) {
        return;
      }
      isSensing = false;
      animationWrapper.StartNewAnimation(DeactivateSense());
    }
    
    
    private IEnumerator ActivateSense() {
      UpdateElementsToShow(true);
      UpdateFadeFootprints(true);
      yield return UpdateElementEffects(animationProgress, 1 - animationProgress);
      UpdateFadeFootprints(false);
    }
    
    private IEnumerator DeactivateSense() {
      UpdateFadeFootprints(true);
      yield return UpdateElementEffects(animationProgress, -animationProgress);
      UpdateFadeFootprints(false);
      UpdateElementsToShow(false);
    }
    
    private IEnumerator UpdateElementEffects(float startEffectAmount, float changeEffectAmount) {
      for (var i = 0f; i < senseDelay; i += animationFreq) {
        animationProgress = startEffectAmount + i / senseDelay * changeEffectAmount;
        UpdateElements();
        yield return new WaitForSeconds(animationFreq);
      }
    }

    private void UpdateElementsToShow(bool enable) {
      foreach (var element in elementsToShow) {
        element.gameObject.SetActive(enable);
      }
      foreach (var element in footprints) {
        element.gameObject.SetActive(enable);
      }
    }

    private void UpdateFadeFootprints(bool isFading) {
      foreach (var element in footprints) {
        element.IsFading = isFading;
      }
    }

    private void UpdateElements() {
      foreach (var element in elementsToDarken) {
        var color = Color.Lerp(element.regularColor, element.darkenedColor, animationProgress);
        element.spriteRenderer.color = color;
      }
      
      foreach (var element in elementsToShow) {
        var color = Color.Lerp(element.regularColor, element.showColor, animationProgress);
        element.spriteRenderer.color = color;
      }
      
      foreach (var element in footprints) {
        var currentColor = element.CurrentColor();
        var origin = new Color(currentColor.r, currentColor.g, currentColor.b, 0);
        var color = Color.Lerp(origin, currentColor, animationProgress);
        element.sprite.color = color;
      }
    }
  }
}