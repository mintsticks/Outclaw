using System;
using Managers;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Outclaw.Heist {
  public class AttentionZone : MonoBehaviour {

    [SerializeField] private EnterSoundEffect enterDetectEffect;
    [SerializeField] private OnDetect onDetect = new OnDetect();
    [SerializeField] private OnDetect onDetectStay = new OnDetect();
    [SerializeField] private OnDetect onDetectLoss = new OnDetect();

    [Inject] private ISneakManager sneakManager;
    [Inject] private IHideablePlayer hideablePlayer;

    private bool detectedLastFrame = false;

    public void EnterAttention(GameObject objectInAttention) {
      if (sneakManager.IsSneaking || hideablePlayer.Hidden) {
        return;
      }
      EnterDetect(objectInAttention);
    }

    public void StayAttention(GameObject objectInAttention){
      if (sneakManager.IsSneaking || hideablePlayer.Hidden) {
        onDetectLoss.Invoke();
        detectedLastFrame = false;
        return;
      }

      if(detectedLastFrame){
        onDetectStay.Invoke();
        return;
      }

      EnterDetect(objectInAttention);
    }

    public void ExitAttention(GameObject objectInAttention) {
      onDetectLoss.Invoke();
      detectedLastFrame = false;
    }

    private void EnterDetect(GameObject objectInAttention){
      Vector3 effectLocation = objectInAttention.transform.position;
      Vector3 directionRay = transform.position - effectLocation;
      enterDetectEffect?.Play(effectLocation, directionRay);

      onDetect.Invoke();

      detectedLastFrame = true;
    }
  }
}