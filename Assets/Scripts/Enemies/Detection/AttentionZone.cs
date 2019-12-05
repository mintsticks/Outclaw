﻿using System;
using Managers;
using Outclaw.City;
using UnityEngine;
using UnityEngine.Events;
using Utility;
using Zenject;

namespace Outclaw.Heist {
  public enum AttentionType {
    NONE = 0,
    SOUND = 1,
    SIGHT = 2,
  }

  public class AttentionZone : MonoBehaviour {
    [SerializeField] private EnterSoundEffect enterDetectEffect;
    [SerializeField] private OnDetect onDetect = new OnDetect();
    [SerializeField] private OnDetect onDetectStay = new OnDetect();
    [SerializeField] private OnDetect onDetectLoss = new OnDetect();
    [SerializeField] private AttentionType attentionType = AttentionType.SOUND;
    [SerializeField] private SpriteBundle attentionIndicator;
    [SerializeField] private Capture captureScript;
    
    [SerializeField] private float minIndicatorOpacity = .1f;
    [SerializeField] private float maxIndicatorOpacity = .8f;
    
    [Inject] private ISneakManager sneakManager;
    [Inject] private IHideablePlayer hideablePlayer;
    [Inject] private IPlayerLitManager litManager;
    [Inject] private IPlayer player;

    private bool detectedLastFrame;

    public void EnterAttention(GameObject objectInAttention) {
      if (PlayerIsUndetectable()) {
        return;
      }
      EnterDetect(objectInAttention);
    }

    public void StayAttention(GameObject objectInAttention){
      if (PlayerIsUndetectable()) {
        ExitAttention(objectInAttention);
        return;
      }

      if(detectedLastFrame){
        onDetectStay.Invoke();
        return;
      }

      EnterDetect(objectInAttention);
    }

    private bool PlayerIsUndetectable() {
      switch (attentionType) {
        case AttentionType.SOUND:
          return PlayerIsSilent();
        case AttentionType.SIGHT:
          return PlayerIsNotVisible();
      }
      return false;
    }

    private bool PlayerIsSilent() {
      return sneakManager.IsSneaking || hideablePlayer.Hidden || Math.Abs(player.Velocity.x) < GlobalConstants.TOLERANCE; 
    }

    private bool PlayerIsNotVisible() {
      if(hideablePlayer.Hidden){
        return true;
      }

      if (litManager.IsLit) {
        return false; 
      }

      return sneakManager.IsSneaking;
    }
    
    public void ExitAttention(GameObject objectInAttention) {
      if (!detectedLastFrame) {
        return;
      }
      onDetectLoss.Invoke();
      SetDetectedState(false);
    }

    private void EnterDetect(GameObject objectInAttention){
      Vector3 effectLocation = objectInAttention.transform.position;
      Vector3 directionRay = transform.position - effectLocation;
      enterDetectEffect?.Play(effectLocation, directionRay);

      onDetect.Invoke();
      SetDetectedState(true);
    }

    private void SetDetectedState(bool detected) {
      detectedLastFrame = detected;
      UpdateAwarenessRate(detected);
      SetIndicatorOpacity(detected);
    }

    private void UpdateAwarenessRate(bool detected) {
      if (detected) {
        captureScript.RegisterAwarenessType(attentionType);
        return;
      }
      captureScript.DeregisterAwarenessType(attentionType);
    }
    
    private void SetIndicatorOpacity(bool detected) {
      if (attentionIndicator == null) {
        return;
      }
      attentionIndicator.SetAlpha(detected ? maxIndicatorOpacity : minIndicatorOpacity);
    }
  }
}