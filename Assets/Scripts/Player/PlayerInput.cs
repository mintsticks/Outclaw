using UnityEngine;
using Zenject;

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
    bool IsStart();
    bool IsStartDown();
    bool IsMenuSubmitDown();
  }

  public class PlayerInput : IPlayerInput, ITickable {
    private bool isLeftPressed;
    private bool isLeftOnce;
    
    private bool isRightPressed;
    private bool isRightOnce;
    
    private bool isDownPressed;
    private bool isDownOnce;
    
    private bool isUpPressed;
    private bool isUpOnce;
    
    public void Tick() {
      UpdatePress(Input.GetAxisRaw("Vertical") < -0.5f, ref isDownPressed, ref isDownOnce);
      UpdatePress(Input.GetAxisRaw("Vertical") > 0.5f, ref isUpPressed, ref isUpOnce);
      UpdatePress(Input.GetAxisRaw("Horizontal") < 0, ref isLeftPressed, ref isLeftOnce);
      UpdatePress(Input.GetAxisRaw("Horizontal") > 0, ref isRightPressed, ref isRightOnce);
    }

    private void UpdatePress(bool axis, ref bool pressed, ref bool once) {
      if (!axis) {
        pressed = false;
        once = false;
        return;
      }

      if (!pressed) {
        pressed = true;
        once = true;
        return;
      }
      once = false;
    }
    public bool IsLeft() {
      return Input.GetAxisRaw("Horizontal") < 0;
    }

    public bool IsLeftDown() {
      return isLeftOnce;
    }

    public bool IsRight() {
      return Input.GetAxisRaw("Horizontal") > 0;
    }
    
    public bool IsRightDown() {
      return isRightOnce;
    }

    public bool IsUp() {
      return Input.GetAxisRaw("Vertical") > 0.5f;
    }

    public bool IsUpPress() {
      return isUpOnce;
    }
    
    public bool IsDown() {
      return Input.GetAxisRaw("Vertical") < -0.5f;
    }

    public bool IsDownPress() {
      return isDownOnce;
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

    public bool IsStart() {
      return Input.GetButton("Start");
    }

    public bool IsStartDown() {
      return Input.GetButtonDown("Start");
    }

    public bool IsMenuSubmitDown(){
      return Input.GetButtonDown("Submit");
    }
  }
}
