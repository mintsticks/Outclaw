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
    
    private bool isJumping;
    private bool isDescending;

    public void UpdateMovement() {
      UpdateHorizontal();
      UpdateVertical();
      UpdateAnimationState(velocity);
    }

    public void UpdatePhysics() {
      controller.Move(velocity * Time.fixedDeltaTime, ref isJumping, ref isDescending);
      velocity = controller.Velocity;
    }
    
    private void UpdateHorizontal() {
      var moveDir = MoveDirection();
      if (!controller.isGrounded) {
        Debug.Log("im off");
      }
      var dampingFactor = controller.isGrounded ? groundDamping : inAirDamping;
      velocity.x = Mathf.Lerp(velocity.x, moveDir * runSpeed, Time.deltaTime * dampingFactor);

      if (moveDir == 0) {
        return;
      }

      var scale = rb.transform.localScale;
      if ((moveDir < 0 && scale.x > 0) || (moveDir > 0 && scale.x < 0)) {
        rb.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
      }
    }

    private int MoveDirection() {
      if (dialogueManager.IsDialogueRunning) {
        return 0;
      }
      return playerInput.IsLeft() ? -1 : playerInput.IsRight() ? 1 : 0;
    }

    private void UpdateVertical() {
      CheckDescend();
      CheckJump();
      velocity.y += gravity * Time.deltaTime;
    }

    private void CheckJump() {
      if (!controller.isGrounded || dialogueManager.IsDialogueRunning) {
        return;
      }

      if (playerInput.IsJumpDown()) {
        isJumping = true;
      }
      
      velocity.y = isJumping ? Mathf.Sqrt(2f * jumpHeight * -gravity) : velocity.y;
    }

    private void CheckDescend() {
      if (!controller.isGrounded || dialogueManager.IsDialogueRunning) {
        return;
      }
      if (playerInput.IsDownPress()) {
        isDescending = true;
      }
    }

    private void UpdateAnimationState(Vector3 move) {
      ac.SetHorizontalVelocity(Mathf.Abs(move.x));
      ac.SetVerticalVelocity(controller.isGrounded ? 0 : move.y);
      ac.SetIsLanding(controller.IsNearGround(Mathf.Abs(gravity)));
    }
  }
}