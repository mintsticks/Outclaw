﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Outclaw;
using Outclaw.City;
using UnityEngine;
using Utility;
using Zenject;

namespace UI.DismissablePrompts {
  public class ConditionalDisplay : MonoBehaviour {
    [SerializeField] private List<ConditionInfo> conditions;
    [SerializeField] private float fadeTime;
    [SerializeField] private PlatformDependentComponent platformDependentComponent;
    
    [Inject] private IGameStateManager gameStateManager;
    [Inject] private IPlayer player;
    [Inject] private PlatformDependentCanvasFactory canvasFactory;
    
    private IPlatformDependentCanvasComponent canvasComponent;
    private AnimationWrapper animationWrapper;
    private float animationProgress;
    private bool fadingIn;
    private CanvasGroup content;
    
    private void Start() {
      animationWrapper = gameObject.AddComponent<AnimationWrapper>();
      if (platformDependentComponent == null) {
        return;
      }
      canvasComponent = canvasFactory.Create(platformDependentComponent);
      canvasComponent.CanvasGroup.transform.SetParent(transform, false);
      content =  canvasComponent.CanvasGroup;
    }

    private bool AllConditionsMet() {
      return conditions.All(DisplayConditionMet);
    }
    
    private bool DisplayConditionMet(ConditionInfo condition) {
      switch (condition.conditionType) {
        case ConditionType.GAMESTATE:
          return gameStateManager.CurrentGameStateData == condition.gameStateData;
        case ConditionType.TASK:
          return !condition.task.IsComplete;
        case ConditionType.INPUT_ENABLED:
          return !player.InputDisabled;
      }
      return true;
    }
    
    public void Show() {
      if (!AllConditionsMet()|| animationProgress > 1 - GlobalConstants.TOLERANCE) {
        return;
      }
      
      animationWrapper.StartNewAnimation(FadeInAnim());
    }

    public void UpdateCondition() {
      var conditionsMet = AllConditionsMet();
      if (!conditionsMet) {
        animationWrapper.StopCurrentAnimation();
        animationProgress = 0;
        UpdateIndicator();
        fadingIn = false;
        return;
      }

      if (animationProgress < GlobalConstants.TOLERANCE && !fadingIn) {
        animationWrapper.StartNewAnimation(FadeInAnim());
      }
    }
    
    public void Hide() {
      if (animationProgress < GlobalConstants.TOLERANCE) {
        return;
      }
      animationWrapper.StartNewAnimation(FadeOutAnim());
    }

    private IEnumerator FadeInAnim() {
      fadingIn = true;
      yield return UpdateIndicator(animationProgress, 1 - animationProgress);
      fadingIn = false;
    }

    private IEnumerator FadeOutAnim() {
      yield return UpdateIndicator(animationProgress,-animationProgress);
      fadingIn = false;
    }
    
    private IEnumerator UpdateIndicator(float startAmount, float changeAmount) {
      for (var i = 0f; i < fadeTime; i += GlobalConstants.ANIMATION_FREQ) {
        animationProgress = startAmount + i / fadeTime * changeAmount;
        UpdateIndicator();
        yield return new WaitForSeconds(GlobalConstants.ANIMATION_FREQ);
      }
      
      animationProgress = Mathf.Round(animationProgress);
      UpdateIndicator();
    }
    
    private void UpdateIndicator() {
      content.alpha = animationProgress;
    }
  }
  
  [Serializable]
  public class ConditionInfo {
    public ConditionType conditionType;
    public Task task;
    public GameStateData gameStateData;
  }
  
  public enum ConditionType {
    NONE = 0,
    GAMESTATE = 1,
    TASK = 2,
    INPUT_ENABLED = 3
  }
}