using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw.Heist{
  public class GuardAnimationController : MonoBehaviour
  {
    [SerializeField] private Animator anim;
    private float xScale;

    void Awake(){
      xScale = transform.localScale.x;
    }

    public void SetXSpeed(float speed){
      anim.SetFloat("moveSpeed", Mathf.Abs(speed));
      if(speed == 0){
        return;
      }
      Vector3 origScale = transform.localScale;
      origScale.x = (speed < 0) ? xScale : -xScale;
      transform.localScale = origScale;
    }

    public void Turn(){
      transform.localScale = new Vector3(-transform.localScale.x,
        transform.localScale.y, transform.localScale.z);
    }

    public void SetFlashlight(bool on){
      anim.SetBool("hasFlashlight", on);
    }

    // pass in an angle from [0, 90], where 0 is facing forward, 90 is facing the ground
    public void SetFlashlightAngle(float angle){
      anim.SetFloat("flashlightAngle", angle);
    }
  }
}
