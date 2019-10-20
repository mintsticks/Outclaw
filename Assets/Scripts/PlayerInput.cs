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
  }

  public class PlayerInput : IPlayerInput{
    public bool IsLeft() {
      return Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
//      return Input.GetAxis("Horizontal") < 0;
    }

    public bool IsLeftDown() {
      return Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
//      return Input.GetAxis("Horizontal") < 0;
    }

    public bool IsRight() {
      return Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
//      return Input.GetAxis("Horizontal") > 0;
    }
    
    public bool IsRightDown() {
      return Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
//      return Input.GetAxis("Horizontal") > 0;
    }

    public bool IsUp() {
      return Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
//      return Input.GetAxis("Vertical") > 0;
    }

    public bool IsUpPress() {
      return Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
//      return Input.GetAxis("Vertical") > 0;
    }
    
    public bool IsDown() {
      return Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
//      return Input.GetAxis("Vertical") < 0;
    }

    public bool IsDownPress() {
      return Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
//      return Input.GetAxis("Vertical") < 0;
    }
    
    public bool IsJump() {
      return Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
//      return Input.GetAxis("Vertical") > 0;
    }

    public bool IsJumpDown() {
      return Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
//      return Input.GetAxis("Vertical") > 0;
    }

    public bool IsInteract() {
      return Input.GetKey(KeyCode.E);
//      return Input.GetButton("Fire1");
    }
    
    public bool IsInteractDown() {
      return Input.GetKeyDown(KeyCode.E);
//      return Input.GetButtonDown("Fire1");
    }

    public bool IsAbility() {
      return Input.GetKey(KeyCode.R);
//      return Input.GetButton("Fire3");
    }

    public bool IsSense() {
      return Input.GetKey(KeyCode.F);
//      return Input.GetButton("Fire2");
    }
    
    public bool IsSenseDown() {
      return Input.GetKeyDown(KeyCode.F);
//      return Input.GetButtonDown("Fire2");
    }
    
    public bool IsSenseUp() {
      return Input.GetKeyUp(KeyCode.F);
//      return Input.GetButtonUp("Fire2");
    }

    public bool IsPauseDown() {
      return Input.GetKeyDown(KeyCode.Escape);
//      return Input.GetButtonDown("Pause");
    }
  }
}