using City;
using Outclaw.City;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Outclaw {
  public interface IGameStateManager {
    GameStateData CurrentGameStateData { get; }
    GameStateList StateList { get; }
    void SetGameState(GameStateData state, bool persist = false);
  }
  
  public class GameStateManager : IInitializable, IGameStateManager {

    private GameStateData currentGameStateData;

    [Inject]
    private ILocationManager locationManager;

    public GameStateData CurrentGameStateData => currentGameStateData;

    public GameStateList StateList { get; private set; }
    
    public void SetGameState(GameStateData state, bool persist = false) {
      if(state == null){
        Debug.LogError("Null game state passed in.");
        return;
      }

      currentGameStateData = state;
      if (persist) {
        return;
      }
      locationManager.ResetObjectProgress();
    }

    public void Initialize() {
      //TODO(dwong): load from save state.

      StateList = Resources.Load<GameStateList>("Game State Data/Game State List");
      currentGameStateData = StateList[0];
    }
  }
}