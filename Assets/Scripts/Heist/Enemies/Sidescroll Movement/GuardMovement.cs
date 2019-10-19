#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw.Heist{
  public class GuardMovement : MonoBehaviour
  {

    [SerializeField] private LayerMask blurLayer;

    [Tooltip("Optional to fill, vision cone functions will do nothing if blank.")]
    [SerializeField] private VisionCone visionCone;
    [Tooltip("Optional to fill, vision cone functions will do nothing if blank.")]
    [SerializeField] private Transform visionConeTransform;
    [SerializeField] private float speed = 1;
    [SerializeField] [Range(0, 1)] 
    [Tooltip("As a fraction of the speed")]
    private float minSpeedToTurn = .1f;

    [Header("Component Links")]
    [Tooltip("Optional to fill, animation actions will not update if blank.")]
    [SerializeField] private GuardAnimationController anim;

    public Quaternion VisionRotation { get => visionConeTransform.rotation; }

    void Start(){
      anim?.SetFlashlight(visionCone != null);
    }

    public void UpdateVisionCone(Vector3 facingDir){
      if(visionCone == null || visionConeTransform == null){
        return;
      }

      visionConeTransform.rotation = Quaternion.LookRotation(
        Vector3.forward, facingDir);
    }

    public void MoveTowards(Vector3 position, float dt){
      Vector3 moveDir = Vector3.Normalize(position - transform.position);
      Vector3 velocity = moveDir * speed;
      transform.position += velocity * dt;
      anim?.SetXSpeed(velocity.x);
    }

    public void TurnBody(){
      anim?.Turn();
    }

    private bool ContainsLayer(LayerMask layers, int test){
      return ((1 << test) & layers.value) != 0;
    }

    public IEnumerator TurnHead(Quaternion destination, float duration){
      if(visionCone == null || visionConeTransform == null){
        yield break;
      }

      Quaternion start = visionConeTransform.rotation;
      float totalTime = 0;
      while(totalTime < duration){
        totalTime += Time.deltaTime;
        visionConeTransform.rotation = Quaternion.Lerp(start, destination, 
          totalTime / duration);
        yield return null;
      }
      yield break;
    }

    public void ToggleVision(bool on){
      if(visionCone == null){
        return;
      }

      visionCone.gameObject.SetActive(on);
      anim?.SetFlashlight(visionCone != null);
    }

    void OnTriggerEnter2D(Collider2D other){
      if(visionCone != null && ContainsLayer(blurLayer, other.gameObject.layer)){
        visionCone.SetVisible(false);
      }
    }

    void OnTriggerExit2D(Collider2D other){
      if(visionCone != null && ContainsLayer(blurLayer, other.gameObject.layer)){
        visionCone.SetVisible(true);
      }
    }
  }
}
