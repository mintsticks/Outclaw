using Outclaw;
using UnityEngine;
using Utility;

namespace Player {
  public class AnimationController : MonoBehaviour {
    [SerializeField]
    private PlayerAnimController ac;
    
    [SerializeField] 
    private Rigidbody2D rb;

    public void UpdateAnimationState(Vector3 move, CharacterController2D controller) {
      ac.SetHorizontalVelocity(Mathf.Abs(move.x));
      ac.SetVerticalVelocity(controller.isGrounded ? 0 : move.y);
      ac.SetIsLanding(move.y <= 0 && controller.IsNearGround(Mathf.Abs(GlobalConstants.GRAVITY)));
    }

    public void TurnCharacter(bool turnLeft) {
      var scale = rb.transform.localScale;
      if (turnLeft && scale.x > 0 || !turnLeft && scale.x < 0) {
        rb.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
      }
    }
  }
}