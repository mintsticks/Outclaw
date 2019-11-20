using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw.Heist{
  public class EnterSoundEffect : MonoBehaviour
  {
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private ParticleSystem otherParticle;

    [Header("Mask")]
    [SerializeField] private Transform extentionParent;
    [SerializeField] private Transform tip;

    public void Play(Vector3 position, Vector3 directionRay){
      particle.Play();
      otherParticle.Play();
      UpdateMask(position, directionRay);
    }

    private void UpdateMask(Vector3 position, Vector3 directionRay){
      transform.position = position;

      // set direction
      transform.rotation = Quaternion.LookRotation(Vector3.forward, directionRay);

      // set length
      float distance = directionRay.magnitude;
      extentionParent.localScale = new Vector3(
        extentionParent.localScale.x,
        distance,
        extentionParent.localScale.y);
      tip.position = position + directionRay;
    }
  }
}
