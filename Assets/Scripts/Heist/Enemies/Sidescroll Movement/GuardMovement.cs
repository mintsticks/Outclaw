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
      transform.position += moveDir * speed * dt;
    }

    private bool ContainsLayer(LayerMask layers, int test){
      return ((1 << test) & layers.value) != 0;
    }

    public IEnumerator TurnHead(Quaternion destination, float duration){
      Quaternion start = visionCone.transform.rotation;
      float totalTime = 0;
      while(totalTime < duration){
        totalTime += Time.deltaTime;
        visionCone.transform.rotation = Quaternion.Lerp(start, destination, 
          totalTime / duration);
        yield return null;
      }
      yield break;
    }

    void OnTriggerEnter2D(Collider2D other){
      if(ContainsLayer(blurLayer, other.gameObject.layer)){
        visionCone.SetVisible(false);
      }
    }

    void OnTriggerExit2D(Collider2D other){
      if(ContainsLayer(blurLayer, other.gameObject.layer)){
        visionCone.SetVisible(true);
      }
    }
  }
}
