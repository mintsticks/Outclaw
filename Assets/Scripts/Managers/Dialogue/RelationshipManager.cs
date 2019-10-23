using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public interface IRelationshipManager {
    int GetRankForCat(CatType type);
    int GetRankForCatInGameState(CatType type, GameStateType state);
    void RankUpCat(CatType type);
    void RankUpCatInGameState(CatType type, GameStateType state);
    void LoadRelationshipState();
  }
  
  public class RelationshipManager : IInitializable, IRelationshipManager {
    private Dictionary<CatType, int> catRanks;
    private Dictionary<CatType, Dictionary<GameStateType, int>> gameStateProgress;

    public void Initialize() {
      LoadRelationshipState();
    }

    public int GetRankForCat(CatType type) {
      CheckRankForCat(type);
      return catRanks[type];
    }

    public int GetRankForCatInGameState(CatType type, GameStateType state) {
      CheckRankForCatInGameState(type, state);
      return gameStateProgress[type][state];
    }

    public void RankUpCat(CatType type) {
      CheckRankForCat(type);
      catRanks[type]++;
    }
    
    public void RankUpCatInGameState(CatType type, GameStateType state) {
      CheckRankForCatInGameState(type, state);
      gameStateProgress[type][state]++;
    }

    private void CheckRankForCat(CatType type) {
      if (!catRanks.ContainsKey(type)) {
        catRanks.Add(type, 0);
      }
    }

    private void CheckRankForCatInGameState(CatType type, GameStateType state) {
      if (!gameStateProgress.ContainsKey(type)) {
        gameStateProgress.Add(type, new Dictionary<GameStateType, int>());
      }
      var stateForCat = gameStateProgress[type];
      if (!stateForCat.ContainsKey(state)) {
        stateForCat.Add(state, 0);
      }
    }

    public void LoadRelationshipState() {
      catRanks = new Dictionary<CatType, int>();
      gameStateProgress = new Dictionary<CatType, Dictionary<GameStateType, int>>();
      //TODO(dwong): add rank based on saved state.
    }
  }
  
  public enum CatType {
    NONE = 0,
    CROOK = 1,
    BEAUTY = 2,
    JOKER = 3,
    SLIM = 4
  }

  // Wrapper class for our conversation list, so it can be serialized.
  [Serializable]
  public class SerializedDialogue {
    public TextAsset[] dialogue;
  }
}