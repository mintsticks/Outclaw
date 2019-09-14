using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public class Player : MonoBehaviour {
    [SerializeField] 
    private Rigidbody2D rb;

    [SerializeField]
    private float walkSpeed;

    [SerializeField] 
    private float jumpForce;
    
    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private LayerMask interactableLayer;
    
    [SerializeField]
    private SpriteRenderer sprite;

    [SerializeField]
    private PlayerAnimController ac;

    [Inject]
    private IPlayerData playerData;
    
    private bool isWaiting;
    private bool isJumping;
    private Interactable interactable;

    public void Awake() {
      playerData.Load();
    }

    void FixedUpdate() {
      UpdateHorizontal();
      UpdateVertical();
      UpdateInteraction();
      playerData.Save();
      Debug.Log(playerData.Name + " " + playerData.CatCount);
    }

    private void UpdateInteraction() {
      if (GetInteract() && interactable != null) {
        interactable.Interact();
      }
    }
    
    private void UpdateVertical() {
      if (rb.velocity.y > 0) {
        return;
      }
      if (GetJump() && IsGrounded() && !isJumping) {
        Jump();
      }
    }

    private void Jump() {
      playerData.Name = "Robby";
      playerData.CatCount++;
      rb.velocity = new Vector2(rb.velocity.x, jumpForce);
      isJumping = true;
    }

    private void UpdateHorizontal() {
      var moveDir = GetMoveDirection();
      if (moveDir == 0) {
        ac.SetHorizontalVelocity(0);
        return;
      }
      
      transform.right = new Vector2(moveDir, 0);
      ac.SetHorizontalVelocity(walkSpeed);
      transform.Translate(new Vector2(moveDir * walkSpeed * Time.fixedDeltaTime, 0),Space.World);
    }

    private bool IsGrounded() {
      var epsilon = .1f;
      var spriteBounds = sprite.bounds;
      var boxSize = new Vector2((spriteBounds.max.x - spriteBounds.min.x) / 2, epsilon);
      var boxPos = new Vector2(transform.position.x, spriteBounds.min.y - epsilon / 2);
      return Physics2D.OverlapBox(boxPos, boxSize, 0, groundLayer);
    }

    private bool GetJump() {
      return Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
    }
        
    private int GetMoveDirection() {
      if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
        return -1;
      }

      if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
        return 1;
      }

      return 0;
    }
    
    private bool GetInteract() {
      return Input.GetKey(KeyCode.E);
    }
    
    void OnCollisionStay2D(Collision2D other) {
      if ((1 << other.gameObject.layer & groundLayer) == 0) return;
      if (isJumping && IsGrounded()) {
        isJumping = false;
      }
    }

    private void OnTriggerEnter2D(Collider2D other) {
      if ((1 << other.gameObject.layer & interactableLayer) == 0) return;
      interactable = other.GetComponent<Interactable>();
      interactable.InRange();
    }
    
    private void OnTriggerExit2D(Collider2D other) {
      if ((1 << other.gameObject.layer & interactableLayer) == 0) return;
      other.GetComponent<Interactable>().ExitRange();
      interactable = null;
    }
  }
}