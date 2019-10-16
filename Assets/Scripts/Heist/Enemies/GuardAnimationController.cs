using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw.Heist{
  public class GuardAnimationController : MonoBehaviour
  {
    [SerializeField] private Animator anim;
    private float xScale;

    void Start(){
      xScale = transform.localScale.x;
    }

    public void SetXSpeed(float speed){
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
  }
}
