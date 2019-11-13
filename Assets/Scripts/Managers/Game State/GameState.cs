using System;
using System.Collections.Generic;
using System.Linq;
using City;
using Outclaw.City;

namespace Outclaw {  
  [Serializable]
  public class GameState {
    public List<GameStateObjectives> childStates;
  }

  [Serializable]
  public class GameStateObjectives {
    public GameStateData nextStateData;
    public List<Task> tasks;
    public bool persistObjectiveState;

    public bool HasAllTasksComplete{
      get => tasks.All(task => task.IsComplete);
    }
  }
}