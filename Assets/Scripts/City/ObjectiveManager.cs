using System;
using System.Collections.Generic;
using System.Linq;
using Outclaw;
using Outclaw.City;
using UnityEngine;
using Zenject;

namespace City {
  public interface IObjectiveManager {
    void CompleteObjective(ObjectType objective);
    bool GameStateObjectivesComplete();
  }
  
  public class ObjectiveManager : MonoBehaviour, IObjectiveManager {
    [SerializeField]
    private List<GameStateObjectives> objectivesForGameStates;

    [Inject]
    private IGameStateManager gameStateManager;
    
    private Dictionary<GameState, HashSet<ObjectType>> completedObjectives;

    private void Awake() {
      completedObjectives = new Dictionary<GameState, HashSet<ObjectType>>();
    }
     
    public void CompleteObjective(ObjectType type) {
      var currentState = gameStateManager.CurrentGameState;
      if (!completedObjectives.ContainsKey(currentState)) {
        completedObjectives.Add(currentState, new HashSet<ObjectType>());
      }
      
      completedObjectives[currentState].Add(type);
    }

    public bool GameStateObjectivesComplete() {
      var currentState = gameStateManager.CurrentGameState;
      if (!completedObjectives.ContainsKey(currentState)) {
        return false;
      }

      var gameStateObjectives = objectivesForGameStates.FirstOrDefault(gso => gso.state == currentState);
      return gameStateObjectives == null || completedObjectives[currentState].SetEquals(gameStateObjectives.objectives);
    }
  }

  [Serializable]
  public class GameStateObjectives {
    public GameState state;
    public List<ObjectType> objectives;
  }
}