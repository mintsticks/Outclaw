using Zenject;

namespace Outclaw {
  public interface IGameStateManager {
    GameState CurrentGameState { get; set; }
  }
  
  public class GameStateManager : IInitializable, IGameStateManager {
    private GameState currentGameState;

    public GameState CurrentGameState {
      get { return currentGameState; }
      set { currentGameState = value; }
    }

    public void Initialize() {
      //TODO(dwong): load from save state.
      currentGameState = GameState.TUTORIAL;
    }
  }

  public enum GameState {
    NONE,
    TUTORIAL,
    CITY,
  }
}