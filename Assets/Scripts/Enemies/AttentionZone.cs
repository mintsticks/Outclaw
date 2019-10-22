using System;
using Managers;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public class AttentionZone : MonoBehaviour {
    [SerializeField] private OnDetect onDetect = new OnDetect();

    [Inject] private ISneakManager sneakManager;
    [Inject] private IHideablePlayer hideablePlayer;
    public void EnterAttention() {
      if (sneakManager.IsSneaking || hideablePlayer.Hidden) {
        return;
      }
      onDetect.Invoke();
    }
  }
}