using System;
using System.Collections.Generic;
using System.Linq;
using Outclaw;
using Outclaw.City;
using UnityEngine;
using Zenject;

namespace City {
  public interface IObjectiveManager {
    Task CurrentTask { get; }
    void CompleteTask(Task task);
    void UpdateCurrentTask();
    void UpdateGameState();
  }
  
  public class ObjectiveManager : MonoBehaviour, IObjectiveManager {
    [SerializeField]
    private List<GameState> objectiveInfos;

    [Inject]
    private IGameStateManager gameStateManager;
    
    private Task currentTask;
    public Task CurrentTask { get => currentTask; }

    public void CompleteTask(Task task){
      task.Complete(gameStateManager.CurrentGameStateData);
      UpdateCurrentTask();
      UpdateGameState();
    }

    public void UpdateCurrentTask(){
      var currentState = gameStateManager.CurrentGameStateData;
      var currentChild = currentState.childStates.FirstOrDefault(child => !IsTasksComplete(child.tasks));
      currentTask = currentChild?.tasks.FirstOrDefault(task => !task.IsComplete(currentState));
    }

    private bool IsTasksComplete(List<Task> tasks){
      return tasks.All(task => task.IsComplete(gameStateManager.CurrentGameStateData));
    }

    public void UpdateGameState() {
      var currentState = gameStateManager.CurrentGameStateData;
      
      foreach (var child in currentState.childStates.Where(child => IsTasksComplete(child.tasks))) {
        gameStateManager.SetGameState(child.nextStateData, child.persistObjectiveState);
        break;
      }
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