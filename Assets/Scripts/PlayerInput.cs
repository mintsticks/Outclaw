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
    bool IsDown();
    
    bool IsJump();
    bool IsInteract();
    bool IsInteractDown();
    bool IsAbility();
    bool IsSense();

    bool IsPauseDown();
  }

  public class PlayerInput : IPlayerInput{
    public bool IsLeft() {
      return Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
    }

    public bool IsLeftDown() {
      return Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
    }

    public bool IsRight() {
      return Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
    }
    
    public bool IsRightDown() {
      return Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
    }

    public bool IsUp() {
      return Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
    }

    public bool IsDown() {
      return Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
    }

    public bool IsJump() {
      return Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
    }

    public bool IsInteract() {
      return Input.GetKey(KeyCode.E);
    }
    
    public bool IsInteractDown() {
      return Input.GetKeyDown(KeyCode.E);
    }

    public bool IsAbility() {
      return Input.GetKey(KeyCode.R);
    }

    public bool IsSense() {
      return Input.GetKey(KeyCode.F);
    }

    public bool IsPauseDown() {
      return Input.GetKeyDown(KeyCode.Escape);
    }
  }
}