using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Managers;
using Outclaw.City;
using UnityEngine;
using Utility;
using Zenject;

namespace Outclaw.Heist {
  public class Capture : MonoBehaviour {
    [Header("Awareness")] 
    [SerializeField] private float cooldownAwarenessRate = -.25f;
    [SerializeField] private float cooldownAwarenessTime = .5f;
    [SerializeField] private AttentionTypeAwarenessData attentionTypeAwarenessData;
    [SerializeField] private SpriteRenderer alertSprite;

    [Inject] private ICapturedMenu captureMenu;
    [Inject] private IPlayer player;
    [Inject] private IHeistInteractionController interactionController;
    [Inject] private IPlayerCapturedManager playerCapturedManager;

    private float currentAwareness;
    private float currentAwarenessRate;
    private float currentPercentChange;
    private float maxAwareness = 1f;
    private float timeSinceAware;

    void Update() {
      UpdateAwareness();
      UpdateColor();
      CheckCapture();
    }

    private void UpdateAwareness() {
      UpdateTimeSinceAware();
      var awarenessRateInPercent = timeSinceAware > cooldownAwarenessTime ? cooldownAwarenessRate : currentAwarenessRate;
      var awarenessChangePerSecond = maxAwareness * awarenessRateInPercent;
      currentAwareness = currentAwareness.ClampedAdd(awarenessChangePerSecond * Time.deltaTime, 0, maxAwareness);
    }

    private void UpdateTimeSinceAware() {
      if (currentAwarenessRate.IsZero()) {
        timeSinceAware += Time.deltaTime;
        return;
      }

      timeSinceAware = 0;
    }
    
    private void UpdateColor() {
      var alpha = currentAwareness / maxAwareness;
      alertSprite.color = alertSprite.color.WithAlpha(alpha);
    }

    private void CheckCapture() {
      if (currentAwareness < maxAwareness || playerCapturedManager.IsCaptured) {
        return;
      }

      playerCapturedManager.IsCaptured = true;
      StartCoroutine(CaptureCoroutine());
    }

    public void RegisterAwarenessType(AttentionType type) {
      currentAwarenessRate = currentAwarenessRate.ClampedAdd(GetAwarenessRateChangeForType(type), 0, 1);
    }
    
    public void DeregisterAwarenessType(AttentionType type) {
      currentAwarenessRate = currentAwarenessRate.ClampedAdd(-GetAwarenessRateChangeForType(type), 0, 1);
    }

    private float GetAwarenessRateChangeForType(AttentionType type) {
      var info = attentionTypeAwarenessData.data.FirstOrDefault(d => d.attentionType == type);
      var change = info?.awarenessRateChange;
      return change.GetValueOrDefault();
    }
    
    private IEnumerator CaptureCoroutine() {
      player.InputDisabled = true;
      yield return new WaitForSeconds(.25f);
      CapturePlayerImmediate();
    }

    [UsedImplicitly]
    public void CapturePlayerImmediate() {
      interactionController.ClearInteractable();
      currentAwareness = 0;
      playerCapturedManager.IsCaptured = false;
      captureMenu.Show();
    }
  }

  [Serializable]
  public class AttentionTypeAwarenessData {
    public List<AttentionTypeAwarenessInfo> data;
  }

  [Serializable]
  public class AttentionTypeAwarenessInfo {
    public AttentionType attentionType;
    public float awarenessRateChange;
  }
}
