﻿using System.Collections;
using Outclaw;
using UnityEngine;
using Zenject;

namespace City {
  public interface IHeistSenseManager {
    bool IsSensing { get; }
    bool IsSenseDown { get; }
    bool IsSenseUp { get; }
    void RegisterSpriteToGrey(SpriteRenderer spriteRenderer);
  }
  
  public class HeistSenseManager : MonoBehaviour {
    [SerializeField] private float senseDelay;
    [SerializeField] private AnimationWrapper animationWrapper;
    [Inject] private IPlayerInput playerInput;
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
    
    
    private IEnumerator ActivateSense() {
      yield break;;
    }

    private IEnumerator DeactivateSense() {
      yield break;
    }
  }
}