﻿using System;
using Outclaw;
using UnityEngine;
using Zenject;

namespace Managers {
  public interface ISneakManager {
    bool IsSneaking { get; }
    bool IsSneakingDown { get; }
    bool IsSneakingUp { get; }
  }
  
  public class SneakManager : MonoBehaviour, ISneakManager {
    [SerializeField] private SneakVisual sneakVisual;
    [SerializeField] private float maxSneakSeconds;
    [SerializeField] private float sneakCooldownFactor = .75f;
    [SerializeField] private float warningPercent = .8f;
    [Inject] private IPlayerInput playerInput;
    
    public bool IsSneaking => isSneaking;
    public bool IsSneakingDown => isSneakingDown;
    public bool IsSneakingUp => isSneakingUp;
    
    private bool isSneaking;
    private bool isSneakingDown;
    private bool isSneakingUp;

    private float sneakSeconds;
    private bool visible = true;
    private bool pulsing;
    private void Update() {
      CheckSneakDown();
      CheckSneakUp();
      UpdateSneak();
      UpdateSneakVisual();
      UpdatePulse();
    }

    private void CheckSneakDown() {
      if (!playerInput.IsSneakDown()) {
        isSneakingDown = false;
        return;
      }

      isSneaking = true;
      isSneakingDown = true;
    }

    private void UpdateSneakVisual() {
      sneakVisual.SetSneakPercent(1 - sneakSeconds/maxSneakSeconds);
    }

    private void UpdateSneak() {
      if (!isSneaking) {
        CooldownSneak();
        return;
      }
      UseSneak();
    }

    private void CooldownSneak() {
      var cooledSneak = sneakSeconds - Time.deltaTime * sneakCooldownFactor;
      sneakSeconds = Mathf.Max(0, cooledSneak);
      if (Math.Abs(sneakSeconds) < .00001f && visible) {
        sneakVisual.DelayedFadeOut();
        visible = false;
      }
    }

    private void UseSneak() {
      if (Math.Abs(sneakSeconds) < .00001f && !visible) {
        sneakVisual.FadeIn();
        visible = true;
      }
      sneakSeconds += Time.deltaTime;
      CheckSneakLimit();
    }

    private void UpdatePulse() {
      CheckPulseOn();
      CheckPulseOff();
    }

    private void CheckPulseOn() {
      if (!(sneakSeconds / maxSneakSeconds > warningPercent) || pulsing) {
        return;
      }
      sneakVisual.Pulse();
      pulsing = true;
    }

    private void CheckPulseOff() {
      if (!(sneakSeconds / maxSneakSeconds <= warningPercent) || !pulsing) {
        return;
      }
      pulsing = false;
      sneakVisual.StopAnimation();
      sneakVisual.SetVisible();
    }
    
    private void CheckSneakLimit() {
      if (!(sneakSeconds >= maxSneakSeconds)) {
        return;
      }
      DisableSneak();
    }
    
    private void CheckSneakUp() {
      if (!playerInput.IsSneakUp()) {
        isSneakingUp = false;
        return;
      }
      DisableSneak();
    }

    private void DisableSneak() {
      isSneaking = false;
      isSneakingUp = true;
    }
  }
}