#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw.Heist{
  public class GuardMovement : MonoBehaviour
  {

    [SerializeField] private LayerMask blurLayer;
    [SerializeField] private VisionCone visionCone;
    [SerializeField] private float speed = 1;
    private Vector3 prevPosition;

    public void UpdateVisionCone(Vector3 facingDir){
      visionCone.gameObject.transform.rotation = Quaternion.LookRotation(
        Vector3.forward, facingDir);
    }

    public void MoveTowards(Vector3 position, float dt){
      Vector3 moveDir = Vector3.Normalize(position - transform.position);
      transform.position += moveDir * speed;
    }

    void OnTriggerEnter2D(Collider2D other){
      if(other.gameObject.layer == blurLayer){
        visionCone.SetVisible(false);
      }
    }

    void OnTriggerExit2D(Collider2D other){
      if(other.gameObject.layer == blurLayer){
        visionCone.SetVisible(true);
      }
    }
  }
}
