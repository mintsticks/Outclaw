﻿using System;
using System.Collections.Generic;
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
  }

  [Serializable]
  public class ConversationObjective {
    public CatType type;
    public int requiredConversationCount;
  }
}