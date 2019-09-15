using UnityEngine;

namespace Outclaw {
  /// <summary>
  /// Helper class to detect user input.
  /// </summary>
  public interface IPlayerInput {
    bool IsLeft();
    bool IsRight();
    bool IsUp();
    bool IsDown();
    
    bool IsJump();
    bool IsInteract();
    bool IsAbility();
    bool IsSense();
  }

  public class PlayerInput : IPlayerInput{
    public bool IsLeft() {
      return Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
    }

    public bool IsRight() {
      return Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
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

    public bool IsAbility() {
      return Input.GetKey(KeyCode.R);
    }

    public bool IsSense() {
      return Input.GetKey(KeyCode.F);
    }
  }
}