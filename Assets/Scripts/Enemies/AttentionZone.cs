using System;
using Managers;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Outclaw.Heist {
  public class AttentionZone : MonoBehaviour {
    [SerializeField] private OnDetect onDetect = new OnDetect();
    [SerializeField] private UnityEvent onDetectLoss = new UnityEvent();

    [Inject] private ISneakManager sneakManager;
    [Inject] private IHideablePlayer hideablePlayer;
    public void EnterAttention() {
      if (sneakManager.IsSneaking || hideablePlayer.Hidden) {
        return;
      }
      onDetect.Invoke();
    }

    public void StayAttention(){
      if (sneakManager.IsSneaking || hideablePlayer.Hidden) {
        onDetectLoss.Invoke();
        return;
      }

      onDetect.Invoke();
    }

    public void ExitAttention() {
      onDetectLoss.Invoke();
    }
  }
}