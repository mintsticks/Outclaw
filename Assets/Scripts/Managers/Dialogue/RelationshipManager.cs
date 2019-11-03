using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public interface IRelationshipManager {
    void RankUpCat(CatDialogueData data);
    void RankUpCatInGameState(CatDialogueData data);
    void LoadRelationshipState();
    void ResetRelationships();
  }
  
  public class RelationshipManager : IInitializable, IRelationshipManager{

    private HashSet<CatDialogueData> activeData = new HashSet<CatDialogueData>();

    [Inject] IGameStateManager gameStateManager;

    public void Initialize() {
      LoadRelationshipState();
      gameStateManager.OnNonpersistReset += Reset;
    }

    public void RankUpCat(CatDialogueData data) {
      data.IncreaseRank();
      activeData.Add(data);
    }
    
    public void RankUpCatInGameState(CatDialogueData data) {
      data.IncreateGameStateRank();
      activeData.Add(data);
    }

    public void LoadRelationshipState() {
      ResetRelationships();
      //TODO(dwong): add rank based on saved state.
    }

    public void ResetRelationships(){

      foreach(CatDialogueData data in activeData){
        data.Reset();
      }
      activeData.Clear();
    }

    public void Reset(){
      ResetRelationships();
    }
  }

  // Wrapper class for our conversation list, so it can be serialized.
  [Serializable]
  public class SerializedDialogue {
    public TextAsset[] dialogue;
  }
}