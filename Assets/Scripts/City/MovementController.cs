﻿using System;
using Boo.Lang.Runtime.DynamicDispatching;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  /// <summary>
  /// Updates the players position and appearance in the city.
  /// </summary>
  public class MovementController : MonoBehaviour {
    [SerializeField]
    private CharacterController2D controller;
    
    [SerializeField]
    private PlayerAnimController ac;
    
    [SerializeField] 
    private Rigidbody2D rb;

    [SerializeField] 
    private float jumpHeight;

    [SerializeField]
    private float gravity;

    [SerializeField]
    private float runSpeed;

    [SerializeField]
    private float groundDamping;
    
    [SerializeField]
    private float inAirDamping;
    
    [Inject]
    private IPlayerInput playerInput;

    [Inject]
    private IDialogueManager dialogueManager;
    
    private Vector3 velocity;
    public Vector3 Velocity => velocity;

    public void UpdateMovement() {
      UpdateHorizontal();
      UpdateVertical();
      UpdateAnimationState(velocity);
      controller.move(velocity * Time.deltaTime);
      velocity = controller.Velocity;
    }
    
    private void UpdateHorizontal() {
      var moveDir = MoveDirection();
      var dampingFactor = controller.isGrounded ? groundDamping : inAirDamping;
      velocity.x = Mathf.Lerp(velocity.x, moveDir * runSpeed, Time.deltaTime * dampingFactor);

      if (moveDir == 0) {
        return;
      }
      
      rb.transform.right = new Vector2(moveDir, 0);
    }
    
    private int MoveDirection() {
      if (dialogueManager.IsDialogueRunning) {
        return 0;
      }
      return playerInput.IsLeft() ? -1 : playerInput.IsRight() ? 1 : 0;
    }

    private void UpdateVertical() {
      CheckJump();
      velocity.y += gravity * Time.deltaTime;
    }

    private void CheckJump() {
      if (!controller.isGrounded || dialogueManager.IsDialogueRunning) {
        return;
      }

      velocity.y = playerInput.IsJumpDown() ? Mathf.Sqrt(2f * jumpHeight * -gravity) : 0;
    }

    private void UpdateAnimationState(Vector3 move) {
      ac.SetHorizontalVelocity(Mathf.Abs(move.x));
      ac.SetVerticalVelocity(move.y);
    }
  }
}