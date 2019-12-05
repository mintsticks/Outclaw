#pragma warning disable 649

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    [SerializeField] private Bounds bodyBounds;

    [Header("Component Links")]
    [Tooltip("Optional to fill, animation actions will not update if blank.")]
    [SerializeField] private GuardAnimationController anim;
    [Tooltip("Optional")]
    [SerializeField] private Image turnIndicator;

    [Inject] private IPauseGame pause;
    [Inject] private City.IPlayer player;

    // player facing
    private Vector2 currentDir = new Vector2(-1, 0);
    private float currentArmAngle;
    private bool turned;
    private bool facingPlayer = true;

    public Quaternion VisionRotation { get => visionConeTransform.rotation; }
    public Bounds BodyBounds { get => bodyBounds; }


    void Start(){
      anim?.SetFlashlight(visionCone != null);

      if(turnIndicator != null){
        turnIndicator.fillAmount = 0;
      }
    }

    public void UpdateVisionCone(Vector3 lookingDir){
      if(visionCone == null || visionConeTransform == null){
        return;
      }

      visionConeTransform.rotation = Quaternion.LookRotation(
        Vector3.forward, lookingDir);
    }

    public void MoveTowards(Vector3 position, float dt){
      if(!facingPlayer){
        return;
      }

      Vector3 moveDir = Vector3.Normalize(position - transform.position);
      Vector3 velocity = moveDir * speed;
      transform.position += velocity * dt;
      anim?.SetXSpeed(velocity.x);
    }

    public void TurnBody(){
      currentDir.x = -currentDir.x;
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
        if(pause.IsPaused || !facingPlayer){
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

    public IEnumerator Turn(Vector3 leftDir, Vector3 rightDir, bool startingLeft,
        float lookPause, float turnTime, float visionAngle, Action finishCallback = null){

      MoveTowards(transform.position, 0);

      Vector3 endDir = startingLeft ? leftDir : rightDir;
      Quaternion bottomRot = Quaternion.AngleAxis(180f, Vector3.forward);
      Quaternion endRot =  Quaternion.LookRotation(Vector3.forward, endDir);
      yield return WaitForTurn(lookPause);
      yield return TurnVision(bottomRot, turnTime, visionAngle, false);

      // stall because in facing player
      while(!facingPlayer){
        yield return null;
      }

      TurnBody();
      yield return TurnVision(endRot, turnTime, visionAngle, true);

      finishCallback?.Invoke();
      yield break;
    }

    private IEnumerator WaitForTurn(float waitTime){
      for(float time = 0; time < waitTime; time += Time.deltaTime){
        if(!facingPlayer){
          time -= Time.deltaTime;
        }

        if(turnIndicator != null){
          turnIndicator.fillAmount = time / waitTime;
        }
        yield return null;
      }

      turnIndicator.fillAmount = 0;
    }

    public void SetVision(Quaternion rot){
      visionConeTransform.rotation = rot;
    }

    // pass in angle below horizontal to put arm at (up to 90 degree below)
    public void SetArmAngle(float angle){
      currentArmAngle = Mathf.Clamp(angle, 0, 90);
      anim?.SetFlashlightAngle(currentArmAngle);
    }

    public void ToggleVision(bool on){
      if(visionCone == null){
        return;
      }

      visionCone.gameObject.SetActive(on);
      anim?.SetFlashlight(visionCone != null);
    }

    public void FacePlayer(){
      Vector2 toPlayer = player.PlayerTransform.position - (transform.position 
        + bodyBounds.center);
      float angle = Vector2.Angle(currentDir, toPlayer);

      Debug.Log(angle);
      turned = angle > 90;
      if(turned){
        TurnBody();
        angle = 180 - angle;
      }

      // activate arm anim and point at player
      visionCone?.gameObject.SetActive(false);
      if(visionCone == null){
        anim?.SetFlashlight(true);
      }
      anim?.SetFlashlightAngle(Mathf.Clamp(angle, 0, 90));

      anim?.SetXSpeed(0);
      facingPlayer = false;
    }

    public void UndoFacePlayer(){
      if(turned){
        TurnBody();
      }

      // reset arm to precapture state
      visionCone?.gameObject.SetActive(true);
      if(visionCone == null){
        anim?.SetFlashlight(false);
      }
      anim?.SetFlashlightAngle(currentArmAngle);

      facingPlayer = true;
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
