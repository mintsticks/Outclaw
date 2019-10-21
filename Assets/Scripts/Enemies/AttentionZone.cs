using System;
using UnityEngine;

namespace Outclaw.Heist {
  public class AttentionZone : MonoBehaviour {
    [SerializeField] private OnDetect onDetect = new OnDetect();

    public void EnterAttention() {
      onDetect.Invoke();
    }
  }
}