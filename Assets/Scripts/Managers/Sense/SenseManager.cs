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

    private void UpdateSenseUp() {
      if (!isSensing || !playerInput.IsSenseUp()) {
        isSenseUp = false;
        return;
      }

      isSenseUp = true;
      isSensing = false;
      animationWrapper.StartNewAnimation(senseVisuals.DeactivateSense());
    }
  }
}