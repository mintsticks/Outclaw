using Outclaw;
using UnityEngine;
using Zenject;

public class HeistMovementController : MonoBehaviour {
  [SerializeField]
  private PlayerAnimController ac;

  [SerializeField]
  private Rigidbody2D rb;

  [SerializeField]
  private float walkSpeed;

  [Inject]
  private IPlayerInput playerInput;

  public Rigidbody2D Rigidbody => rb;
  
  public void UpdateMovement() {
    var hDir = GetHorizontalMovement();
    var vDir = GetVerticalMovement();

    var moveDir = new Vector2(hDir, vDir);
    var move = Time.fixedDeltaTime * walkSpeed * moveDir;
    rb.transform.Translate(move,Space.World);

    UpdateAnimationState(hDir);
    
    if (hDir == 0) {
      return;
    }
    rb.transform.right = new Vector2(hDir, 0);

  }

  private int GetVerticalMovement() {
    return playerInput.IsDown() ? -1 : playerInput.IsUp() ? 1 : 0;
  }

  private int GetHorizontalMovement() {
    return playerInput.IsLeft() ? -1 : playerInput.IsRight() ? 1 : 0;
  }
  
  private void UpdateAnimationState(int moveDir) {
    if (moveDir == 0) {
      ac.SetHorizontalVelocity(0);
      return;
    }

    ac.SetHorizontalVelocity(walkSpeed);
  }
}