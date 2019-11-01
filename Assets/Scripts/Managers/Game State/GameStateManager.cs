using System.Collections;
using System.Collections.Generic;
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

    void RegisterAlwaysResetOnStateChange(IResetableManager manager);
    void RegisterNonpersistResetOnStateChange(IResetableManager manager);
  }
  
  public class GameStateManager : IInitializable, IGameStateManager {

    private GameStateData currentGameStateData;

    private HashSet<IResetableManager> alwaysReset = new HashSet<IResetableManager>();
    private HashSet<IResetableManager> nonpersistReset = new HashSet<IResetableManager>();

    public GameStateData CurrentGameStateData => currentGameStateData;

    public GameStateList StateList { get; private set; }
    
    public void SetGameState(GameStateData state, bool persist = false) {
      if(state == null){
        Debug.LogError("Null game state passed in.");
        return;
      }

      currentGameStateData = state;
      
      foreach(IResetableManager manager in alwaysReset){
        manager.Reset();
      }

      if (persist) {
        return;
      }

      foreach(IResetableManager manager in nonpersistReset){
        manager.Reset();
      }
    }

    public void Initialize() {
      //TODO(dwong): load from save state.

      StateList = Resources.Load<GameStateList>("Game State Data/Game State List");
      currentGameStateData = StateList[0];
    }

    public void RegisterAlwaysResetOnStateChange(IResetableManager manager) => alwaysReset.Add(manager);
    public void RegisterNonpersistResetOnStateChange(IResetableManager manager) => nonpersistReset.Add(manager);
  }
}