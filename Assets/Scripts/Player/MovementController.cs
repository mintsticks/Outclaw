using Managers;
using Player;
using UnityEngine;
using Utility;
using Zenject;

namespace Outclaw.City {
  /// <summary>
  /// Updates the players position and appearance in the city.
  /// </summary>
  public class MovementController : MonoBehaviour {
    [SerializeField]
    private CharacterController2D controller;

    [SerializeField]
    private AnimationController animationController;

    [SerializeField] 
    private float jumpHeight;

    [SerializeField]
    private float runSpeed;

    [SerializeField]
    private float groundDamping;
    
    [SerializeField]
    private float inAirDamping;
    
    [Inject]
    private IPlayerInput playerInput;

    [Inject] 
    private IPlayer player;

    private Vector3 inputVelocity;
    private Vector3 velocity;
    public Vector3 Velocity => velocity;
    
    private bool isJumping;
    private bool isDescending;

    public void UpdateMovement() {
      UpdateHorizontal();
      UpdateVertical();
      animationController.UpdateAnimationState(inputVelocity, controller);
    }

    public void UpdatePhysics() {
      controller.Move(inputVelocity * Time.fixedDeltaTime, ref isJumping, ref isDescending);
      velocity = controller.Velocity;
      inputVelocity = controller.Velocity;
    }
    
    private void UpdateHorizontal() {
      var moveDir = MoveDirection();
      var dampingFactor = controller.isGrounded ? groundDamping : inAirDamping;
      inputVelocity.x = Mathf.Lerp(inputVelocity.x, moveDir * runSpeed, Time.deltaTime * dampingFactor);

      if (moveDir == 0) {
        return;
      }

      animationController.TurnCharacter(moveDir < 0);
    }

    private int MoveDirection() {
      if (player.InputDisabled) {
        return 0;
      }
      return playerInput.IsLeft() ? -1 : playerInput.IsRight() ? 1 : 0;
    }

    private void UpdateVertical() {
      CheckDescend();
      CheckJump();
      inputVelocity.y += GlobalConstants.GRAVITY * Time.deltaTime;
    }

    private void CheckJump() {
      if (!controller.isGrounded || player.InputDisabled) {
        return;
      }

      if (playerInput.IsJumpDown()) {
        isJumping = true;
      }
      
      inputVelocity.y = isJumping ? Mathf.Sqrt(2f * jumpHeight * -GlobalConstants.GRAVITY) : inputVelocity.y;
    }

    private void CheckDescend() {
      if (!controller.isGrounded || player.InputDisabled) {
        return;
      }
      if (playerInput.IsDownPress()) {
        isDescending = true;
      }
    }
  }
}
