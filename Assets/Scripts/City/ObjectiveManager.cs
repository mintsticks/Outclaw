﻿using System.Collections.Generic;
using System.Linq;
using Outclaw;
using Outclaw.City;
using UnityEngine;
using Zenject;

namespace City {
  public interface IObjectiveManager {
    void CompleteObjectObjective(ObjectType type);
    void CompleteConversationObjective(CatType type);
    void UpdateGameState();
  }
  
  public class ObjectiveManager : MonoBehaviour, IObjectiveManager {
    [SerializeField]
    private List<GameState> objectiveInfos;
      
    [Inject]
    private IGameStateManager gameStateManager;
    
    private Dictionary<GameStateType, ObjectiveProgress> completedObjectives;

    private void Awake() {
      completedObjectives = new Dictionary<GameStateType, ObjectiveProgress>();
    }
     
    public void CompleteObjectObjective(ObjectType type) {
      var currentState = gameStateManager.CurrentGameState;
      MaybeAddState(currentState);
      completedObjectives[currentState].objects.Add(type);
      UpdateGameState();
    }

    public void CompleteConversationObjective(CatType type) {
      var currentState = gameStateManager.CurrentGameState;
      MaybeAddState(currentState);
      completedObjectives[currentState].conversations.Add(type);
      UpdateGameState();
    }
    
    private void MaybeAddState(GameStateType gameState) {
      if (completedObjectives.ContainsKey(gameState)) {
        return;
      }
      completedObjectives.Add(gameState, new ObjectiveProgress());
    }

    public void UpdateGameState() {
      var currentState = gameStateManager.CurrentGameState;
      var info = objectiveInfos.FirstOrDefault(i => i.currentState == currentState);

      if (info == null) {
        return;
      }
      
      foreach (var child in info.childStates.Where(child => IsObjectivesComplete(child.objectives))) {
        gameStateManager.SetGameState(child.nextState, child.persistObjectiveState);
        break;
      }
    }

    private bool IsObjectivesComplete(List<Objective> objectives) {
      return objectives.All(ObjectiveComplete);
    }

    private bool ObjectiveComplete(Objective objective) {
      var progressForState = completedObjectives[gameStateManager.CurrentGameState];
      switch (objective.objectiveType) {
        case ObjectiveType.CONVERSATION:
          return objective.conversations.All(conv => progressForState.conversations.Contains(conv));
        case ObjectiveType.FIND_OBJECTS:
          return objective.objects.All(obj => progressForState.objects.Contains(obj));
        default:
          return false;
      }
    }
  }

  public class ObjectiveProgress {
    public List<CatType> conversations;
    public List<ObjectType> objects;

    public ObjectiveProgress() {
      conversations = new List<CatType>();
      objects = new List<ObjectType>();
    }
  }
}