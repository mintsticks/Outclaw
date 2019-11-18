using System;
using Managers;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Outclaw.Heist {
  public class AttentionZone : MonoBehaviour {

    [SerializeField] private ParticleSystem enterDetectEffect;
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
      ParticleSystem.EmitParams param = new ParticleSystem.EmitParams();
      param.position = objectInAttention.transform.position;

      Debug.DrawLine(param.position + Vector3.up, param.position + Vector3.down, Color.blue, 100f);
      Debug.DrawLine(param.position + Vector3.left, param.position + Vector3.right, Color.blue, 100f);

      param.applyShapeToPosition = true;
      enterDetectEffect.Emit(param, 1);

      onDetect.Invoke();

      detectedLastFrame = true;
    }
  }
}