namespace Managers {
  public interface IPlayerLitManager {
    bool IsLit { get; set; }
  }
  
  public class PlayerLitManager : IPlayerLitManager {
    private bool isLit;
    public bool IsLit {
      get => isLit;
      set => isLit = value;
    }
  }
}