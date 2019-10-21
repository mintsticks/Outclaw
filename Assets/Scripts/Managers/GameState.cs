using System;
using System.Collections.Generic;
using City;
using Outclaw.City;

namespace Outclaw {  
  [Serializable]
  public class GameState {
    public GameStateType currentState;
    public List<GameStateObjectives> childStates;
  }

  public enum GameStateType {
    NONE = 0,
    TUTORIAL = 1,
    FOUND_KEY = 2,
    FIRST_TIME_CITY = 3,
    FOUND_HANA = 4,
    FOUND_AKI = 5,
    FOUND_COLLAR = 6
  }

  [Serializable]
  public class GameStateObjectives {
    public GameStateType nextState;
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
  
  public enum LocationType {
    NONE,
    HOME,
    MAIN,
    CAFEBOTTOM,
    CAFETOP,
    PARK
  }
}