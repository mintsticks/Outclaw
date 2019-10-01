using System;
using System.Collections.Generic;
using Outclaw;
using Outclaw.City;
using UnityEngine;
using Zenject;

namespace City {
  public class ObjectiveManager : MonoBehaviour {
    [SerializeField]
    private List<GameStateObjectives> objectivesForGameStates;

    [Inject]
    private IGameStateManager gameStateManager;
    
    private Dictionary<GameState, List<ObjectType>> completedObjectives;

    private void Awake() {
      completedObjectives = new Dictionary<GameState, List<ObjectType>>();
    }
     
    public void CompleteObjective(ObjectType type) {
      
    }
  }

  [Serializable]
  public class GameStateObjectives {
    public GameState _state;
    public List<ObjectType> objectives;
  }
}