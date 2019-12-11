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
  
  public class ObjectiveManager : MonoBehaviour, IObjectiveManager{

    [Inject]
    private IGameStateManager gameStateManager;
    
    private Task currentTask;
    private HashSet<Task> completeTasks = new HashSet<Task>();

    public Task CurrentTask => currentTask;

    public void Start(){
      gameStateManager.OnAllReset += Reset;
      UpdateCurrentTask();
    }

    public void CompleteTask(Task task){
      if(task == null){
        return;
      }

      task.Complete();
      completeTasks.Add(task);
      UpdateGameState();
      UpdateCurrentTask();
    }

    public void UpdateCurrentTask(){
      var currentState = gameStateManager.CurrentGameStateData;
      var currentChild = currentState.childStates.FirstOrDefault(child => !child.HasAllTasksComplete);
      currentTask = currentChild?.tasks.FirstOrDefault(task => !task.IsComplete);
    }

    public void UpdateGameState() {
      var currentState = gameStateManager.CurrentGameStateData;
      
      foreach (var child in currentState.childStates.Where(child => child.HasAllTasksComplete)) {
        gameStateManager.SetGameState(child.nextStateData, child.persistObjectiveState);
        break;
      }
    }

    public void Reset(){
      
      foreach(Task t in completeTasks){
        t.Reset();
      }
      completeTasks.Clear();
    }
  }
}