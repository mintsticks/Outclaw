using Outclaw;
using UnityEngine;
using Zenject;

namespace Managers {
  public interface ISneakManager {
    bool IsSneaking { get; }
    bool IsSneakingDown { get; }
    bool IsSneakingUp { get; }
  }
  
  public class SneakManager : ITickable, ISneakManager {
    [Inject] private IPlayerInput playerInput;
    
    public bool IsSneaking => isSneaking;
    public bool IsSneakingDown => isSneakingDown;
    public bool IsSneakingUp => isSneakingUp;
    
    private bool isSneaking;
    private bool isSneakingDown;
    private bool isSneakingUp;
    
    public void Tick() {
      CheckSneakDown();
      CheckSneakUp();
    }

    private void CheckSneakDown() {
      if (!playerInput.IsSneakDown()) {
        isSneakingDown = false;
        return;
      }

      isSneaking = true;
      isSneakingDown = true;
    }
    
    private void CheckSneakUp() {
      if (!playerInput.IsSneakUp()) {
        isSneakingUp = false;
        return;
      }
      isSneaking = false;
      isSneakingUp = true;
    }
  }
}