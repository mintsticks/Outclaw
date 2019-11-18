using System;
using Managers;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Outclaw.Heist {
  public class AttentionZone : MonoBehaviour {
    [SerializeField] private OnDetect onDetect = new OnDetect();
    [SerializeField] private UnityEvent onDetectStay = new UnityEvent();
    [SerializeField] private UnityEvent onDetectLoss = new UnityEvent();

    [Inject] private ISneakManager sneakManager;
    [Inject] private IHideablePlayer hideablePlayer;

    private bool detectedLastFrame = false;

    public void EnterAttention() {
      if (sneakManager.IsSneaking || hideablePlayer.Hidden) {
        return;
      }
      onDetect.Invoke();
      detectedLastFrame = true;
    }

    public void StayAttention(){
      if (sneakManager.IsSneaking || hideablePlayer.Hidden) {
        onDetectLoss.Invoke();
        detectedLastFrame = false;
        return;
      }
      
      if(detectedLastFrame){
        onDetectStay.Invoke();
        return;
      }

      onDetect.Invoke();
      detectedLastFrame = true;
    }

    public void ExitAttention() {
      onDetectLoss.Invoke();
      detectedLastFrame = false;
    }
  }
}