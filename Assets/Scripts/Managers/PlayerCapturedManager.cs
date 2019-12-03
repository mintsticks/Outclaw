namespace Managers {
  public interface IPlayerCapturedManager {
    bool IsCaptured { get; set; }
  }
  
  public class PlayerCapturedManager : IPlayerCapturedManager {
    private bool isCaptured;

    public bool IsCaptured {
      get => isCaptured;
      set => isCaptured = value;
    }
  }

}