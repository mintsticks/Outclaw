using City;
using Outclaw.City;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Outclaw {
  public interface IGameStateManager {
    GameStateType CurrentGameState { get; }
    void SetGameState(GameStateType state, bool persist = false);
  }
  
  public class GameStateManager : IInitializable, IGameStateManager {
    private GameStateType currentGameStateType;

    [Inject]
    private ILocationManager locationManager;

    public GameStateType CurrentGameState => currentGameStateType;
    
    public void SetGameState(GameStateType state, bool persist = false) {
      currentGameStateType = state;
      if (persist) {
        return;
      }
      locationManager.ResetObjectProgress();
    }

    public void Initialize() {
      //TODO(dwong): load from save state.
      currentGameStateType = GameStateType.TUTORIAL;
    }
  }
}