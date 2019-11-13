#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist{
  public class GuardMovement : MonoBehaviour
  {

    [SerializeField] private LayerMask blurLayer;

    [Tooltip("Optional to fill, vision cone functions will do nothing if blank.")]
    [SerializeField] private VisionCone visionCone;
    [Tooltip("Optional to fill, vision cone functions will do nothing if blank.")]
    [SerializeField] private Transform visionConeTransform;
    [SerializeField] private float speed = 1;

    [SerializeField]
    private Bounds bodyBounds;

    [Header("Component Links")]
    [Tooltip("Optional to fill, animation actions will not update if blank.")]
    [SerializeField] private GuardAnimationController anim;

    [Inject] private IPauseGame pause;

    public Quaternion VisionRotation { get => visionConeTransform.rotation; }
    public Bounds BodyBounds { get => bodyBounds; }

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

    /*
     * destination: end rotation
     * duraiton: how long to reach the destination rotation
     * defaultAngle:
     * turningUp: whether the flashlight animation needs to go up or down
     */
    public IEnumerator TurnVision(Quaternion destination, float duration, 
        float defaultAngle = 0, bool turningUp = false){
      if(visionCone == null || visionConeTransform == null){
        yield break;
      }

      Quaternion start = visionConeTransform.rotation;
      float totalTime = 0;
      float angleRange = 90f - defaultAngle;
      while(totalTime < duration){
        if(pause.IsPaused){
          yield return null;
        }

        totalTime += Time.deltaTime;
        visionConeTransform.rotation = Quaternion.Lerp(start, destination, 
          totalTime / duration);

        float progress = totalTime / duration;
        anim?.SetFlashlightAngle(defaultAngle +
          angleRange * (turningUp ? 1 - progress : progress));

        yield return null;
      }
      yield break;
    }

    public void SetVision(Quaternion rot){
      visionConeTransform.rotation = rot;
    }

    // pass in angle below horizontal to put arm at (up to 90 degree below)
    public void SetArmAngle(float angle){
      anim?.SetFlashlightAngle(Mathf.Clamp(angle, 0, 90));
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
