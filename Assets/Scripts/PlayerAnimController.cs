using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{
    
	[SerializeField] private Animator anim;

  public void SetHorizontalVelocity(float val){
  	anim.SetFloat("moveSpeed", val);
  }

  public void SetVerticalVelocity(float val){
    anim.SetFloat("jumpSpeed", val);
  }
}
