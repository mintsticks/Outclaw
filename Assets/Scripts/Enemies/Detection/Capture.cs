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
    [SerializeField] private float defaultAwarenessRatePercent = -.25f;
    [SerializeField] private SpriteRenderer alertSprite;

    [Inject] private ICapturedMenu captureMenu;
    [Inject] private IPlayer player;
    [Inject] private IHeistInteractionController interactionController;
    [Inject] private IPlayerCapturedManager playerCapturedManager;

    private float currentAwareness;
    private float currentAwarenessRate;
    private float currentPercentChange;
    private float maxAwareness = 1f;
    
    private void Start() {
      currentAwarenessRate = defaultAwarenessRatePercent;
    }

    void Update() {
      UpdateAwareness();
      UpdateColor();
      CheckCapture();
    }

    private void UpdateAwareness() {
      var awarenessRateInPercent = currentAwarenessRate.IsZero() ? defaultAwarenessRatePercent : currentAwarenessRate;
      var awarenessChangePerSecond = maxAwareness * awarenessRateInPercent;
      currentAwareness = currentAwareness.ClampedAdd(awarenessChangePerSecond * Time.deltaTime, 0, maxAwareness);
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

    public void AddAwarenessRatePercent(float percentToAdd) {
      currentAwarenessRate = currentAwarenessRate.ClampedAdd(percentToAdd, 0, 1);
    }
    
    private IEnumerator CaptureCoroutine() {
      player.InputDisabled = true;
      yield return new WaitForSeconds(.25f);
      CapturePlayerImmediate();
    }
    
    [UsedImplicitly]
    public void CapturePlayerImmediate(){
      interactionController.ClearInteractable();
      currentAwareness = 0;
      playerCapturedManager.IsCaptured = false;
      captureMenu.Show();
    }
  }
}
