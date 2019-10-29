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
      if(task == null){
        return;
      }

      task.Complete();
      UpdateCurrentTask();
      UpdateGameState();
    }

    public void UpdateCurrentTask(){
      var currentState = gameStateManager.CurrentGameStateData;
      var currentChild = currentState.childStates.FirstOrDefault(child => !IsTasksComplete(child.tasks));
      currentTask = currentChild?.tasks.FirstOrDefault(task => !task.IsComplete);
    }

    private bool IsTasksComplete(List<Task> tasks){
      return tasks.All(task => task.IsComplete);
    }

    public void UpdateGameState() {
      var currentState = gameStateManager.CurrentGameStateData;
      
      foreach (var child in currentState.childStates.Where(child => IsTasksComplete(child.tasks))) {
        gameStateManager.SetGameState(child.nextStateData, child.persistObjectiveState);
        break;
      }
    }
  }
}