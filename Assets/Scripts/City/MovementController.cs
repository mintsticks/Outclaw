using UnityEngine;
using Zenject;

namespace Outclaw.City {
  /// <summary>
  /// Updates the players position and appearance in the city.
  /// </summary>
  public class MovementController : MonoBehaviour {
    [SerializeField]
    private PlayerAnimController ac;
    
    [SerializeField] 
    private Rigidbody2D rb;

    [SerializeField]
    private GroundHelper groundHelper;
    
    [SerializeField]
    private float walkSpeed;
    
    [SerializeField] 
    private float jumpForce;

    [Inject]
    private IPlayerInput playerInput;

    public void UpdateHorizontal() {
      var moveDir = MoveDirection();
      UpdateHorizontalPosition(moveDir);
      UpdateAnimationState(moveDir);
    }
    
    private int MoveDirection() {
      return playerInput.IsLeft() ? -1 : playerInput.IsRight() ? 1 : 0;
    }
    
    private void UpdateHorizontalPosition(int moveDir) {
      if (moveDir == 0) {
        return;
      }

      rb.transform.right = new Vector2(moveDir, 0);
      rb.transform.Translate(new Vector2(moveDir * walkSpeed * Time.fixedDeltaTime, 0),Space.World);
    }

    private void UpdateAnimationState(int moveDir) {
      if (moveDir == 0) {
        ac.SetHorizontalVelocity(0);
        return;
      }
      ac.SetHorizontalVelocity(walkSpeed);
    }
    
    public void UpdateVertical() {
      if (rb.velocity.y > 0 || 
          !playerInput.IsJump() || 
          !groundHelper.IsGrounded()) {
        return;
      }

      rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
  }
}