using System;
using System.Collections.Generic;
using System.Linq;
using Outclaw;
using Outclaw.City;
using UnityEngine;
using Zenject;

namespace City {
  public interface IObjectiveManager {
    void CompleteObjectObjective(ObjectType type);
    void CompleteConversationObjective(CatType type);
    void CompleteEntranceObjective(EntranceType type);
    Objective CurrentObjective { get; set; }
    void UpdateGameState();
    void UpdateCurrentObjective();
  }
  
  public class ObjectiveManager : MonoBehaviour, IObjectiveManager {
    [SerializeField]
    private List<GameState> objectiveInfos;

    [Inject]
    private IGameStateManager gameStateManager;

    private Dictionary<GameStateType, ObjectiveProgress> completedObjectives;
    private Objective currentObjective;
    
    public Objective CurrentObjective {
      get => currentObjective;
      set => currentObjective = value;
    }

    private void Awake() {
      completedObjectives = new Dictionary<GameStateType, ObjectiveProgress>();
    }

    public void CompleteObjectObjective(ObjectType type) {
      var currentState = gameStateManager.CurrentGameState;
      MaybeAddState(currentState);
      CompleteObjective(type, completedObjectives[currentState].objects);
    }

    public void CompleteConversationObjective(CatType type) {
      var currentState = gameStateManager.CurrentGameState;
      MaybeAddState(currentState);
      CompleteObjective(type, completedObjectives[currentState].conversations);
    }

    public void CompleteEntranceObjective(EntranceType type) {
      var currentState = gameStateManager.CurrentGameState;
      MaybeAddState(currentState);
      CompleteObjective(type, completedObjectives[currentState].entrances);
    }

    private void CompleteObjective<T>(T type, ICollection<T> toAddTo) {
      toAddTo.Add(type);
      UpdateCurrentObjective();
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
          return ObjectiveTypeComplete(objective.conversations, progressForState.conversations);
        case ObjectiveType.FIND_OBJECTS:
          return ObjectiveTypeComplete(objective.objects, progressForState.objects);
        case ObjectiveType.USE_ENTRANCE:
          return ObjectiveTypeComplete(objective.entrances, progressForState.entrances);
        default:
          return false;
      }
    }

    private bool ObjectiveTypeComplete<T>(List<T> reqs, List<T> progress) {
      return reqs.All(progress.Contains);
    }
    
    public void UpdateCurrentObjective() {
      var currentState = gameStateManager.CurrentGameState;
      MaybeAddState(currentState);
      var info = objectiveInfos.FirstOrDefault(i => i.currentState == currentState);
      var currentChild = info?.childStates.FirstOrDefault(child => !IsObjectivesComplete(child.objectives));
      currentObjective = currentChild?.objectives.FirstOrDefault(objective => !ObjectiveComplete(objective));
      CurrentObjective = currentObjective;
    }
  }

  public class ObjectiveProgress {
    public List<CatType> conversations;
    public List<ObjectType> objects;
    public List<EntranceType> entrances;

    public ObjectiveProgress() {
      conversations = new List<CatType>();
      objects = new List<ObjectType>();
      entrances = new List<EntranceType>();
    }
  }
}