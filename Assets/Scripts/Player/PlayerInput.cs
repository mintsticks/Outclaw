using UnityEngine;

namespace Outclaw {
  /// <summary>
  /// Helper class to detect user input.
  /// </summary>
  public interface IPlayerInput {
    bool IsLeft();
    bool IsLeftDown();
    bool IsRight();
    bool IsRightDown();
    bool IsUp();
    bool IsUpPress();
    bool IsDown();
    bool IsDownPress();
    bool IsJump();
    bool IsJumpDown();
    bool IsInteract();
    bool IsInteractDown();
    bool IsAbility();
    bool IsSense();
    bool IsSenseDown();
    bool IsSenseUp();
    bool IsPauseDown();
    bool IsSneakDown();
    bool IsSneakUp();
  }

  public class PlayerInput : IPlayerInput{
    public bool IsLeft() {
      return Input.GetAxisRaw("Horizontal") < 0;
    }

    public bool IsLeftDown() {
      return Input.GetAxisRaw("Horizontal") < 0;
    }

    public bool IsRight() {
      return Input.GetAxisRaw("Horizontal") > 0;
    }
    
    public bool IsRightDown() {
      return Input.GetAxisRaw("Horizontal") > 0;
    }

    public bool IsUp() {
      return Input.GetAxisRaw("Vertical") > 0.5f;
    }

    public bool IsUpPress() {
      return Input.GetAxisRaw("Vertical") > 0.5f;
    }
    
    public bool IsDown() {
      return Input.GetAxisRaw("Vertical") < -0.5f;
    }

    public bool IsDownPress() {
      return Input.GetAxisRaw("Vertical") < -0.5f;
    }
    
    public bool IsJump() {
      return Input.GetButton("Jump");
    }

    public bool IsJumpDown() {
      return Input.GetButtonDown("Jump");
    }

    public bool IsInteract() {
      return Input.GetButton("Interact");
    }
    
    public bool IsInteractDown() {
      return Input.GetButtonDown("Interact");
    }

    public bool IsAbility() {
      return Input.GetButton("Ability");
    }

    public bool IsSense() {
      return Input.GetButton("Sense");
    }
    
    public bool IsSenseDown() {
      return Input.GetButtonDown("Sense");
    }
    
    public bool IsSenseUp() {
      return Input.GetButtonUp("Sense");
    }

    public bool IsPauseDown() {
      return Input.GetButtonDown("Pause");
    }

    public bool IsSneakDown() {
      return Input.GetButtonDown("Sneak");
    }

    public bool IsSneakUp() {
      return Input.GetButtonUp("Sneak");
    }
  }
}
