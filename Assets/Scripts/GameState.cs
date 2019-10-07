using System;
using System.Collections.Generic;
using Outclaw.City;

namespace Outclaw {  
  [Serializable]
  public class GameState {
    public GameStateType currentState;
    public List<GameStateObjectives> childStates;
  }

  public enum GameStateType {
    NONE,
    TUTORIAL,
    FIRST_TIME_CITY,
    FOUND_HANA,
    FOUND_AKI
  }

  public enum Location {
    HOUSE,
    CITY,
    CAFE1,
    CAFE2,
    PARK
  }
  
  [Serializable]
  public class GameStateObjectives {
    public GameStateType nextState;
    public List<Objective> objectives;
    public bool persistObjectiveState;
  }
  
  [Serializable]
  public class Objective {
    public Location location;
    public ObjectiveType objectiveType;
    public List<CatType> conversations;
    public List<ObjectType> objects;
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
  }
}