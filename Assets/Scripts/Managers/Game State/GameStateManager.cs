using System;
using System.Collections;
using System.Collections.Generic;
using City;
using Outclaw.City;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Outclaw {

  public delegate void OnReset();

  public interface IGameStateManager {
    GameStateData CurrentGameStateData { get; }
    GameStateList StateList { get; }
    void SetGameState(GameStateData state, bool persist = false);

    event OnReset OnAllReset;
    event OnReset OnNonpersistReset;
  }
  
  public class GameStateManager : IInitializable, IGameStateManager {

    private GameStateData currentGameStateData;

    public GameStateData CurrentGameStateData => currentGameStateData;

    public GameStateList StateList { get; private set; }
    
    public event OnReset OnAllReset;
    public event OnReset OnNonpersistReset;

    public void SetGameState(GameStateData state, bool persist = false) {
      if(state == null){
        Debug.LogError("Null game state passed in.");
        return;
      }

      currentGameStateData = state;
      
      OnAllReset?.Invoke();
      if (persist) {
        return;
      }
      OnNonpersistReset?.Invoke();
    }

    public void Initialize() {
      //TODO(dwong): load from save state.

      StateList = Resources.Load<GameStateList>("Game State Data/Game State List");
      currentGameStateData = StateList[0];
    }
  }
}