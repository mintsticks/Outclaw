using System;
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
    public List<Objective> objectives;
    public bool persistObjectiveState;
  }
  
  [Serializable]
  public class Objective {
    public ObjectiveType objectiveType;
    public List<CatType> conversations;
    public List<ObjectType> objects;
    public List<EntranceType> entrances;
  }

  [Serializable]
  public class ConversationObjective {
    public CatType type;
    public int requiredConversationCount;
  }
  
  public enum ObjectiveType {
    NONE,
    CONVERSATION,
    FIND_OBJECTS,
    USE_ENTRANCE
  }
}