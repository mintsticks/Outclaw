using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Outclaw.City {
  public interface IRelationshipManager {
    int GetRankForCat(CatType type);

    void RankUpCat(CatType type);
    
    TextAsset[] GetDialogueForCat(CatType type);

    void UpdateRelationshipState();
  }
  
  public class RelationshipManager : MonoBehaviour, IRelationshipManager {
    [SerializeField]
    private List<CatInfo> catInfos;
    
    private Dictionary<CatType, int> catRanks;

    public void Awake() {
      UpdateRelationshipState();
    }

    public int GetRankForCat(CatType type) {
      return catRanks[type];
    }

    public void RankUpCat(CatType type) {
      catRanks[type]++;
    }

    public TextAsset[] GetDialogueForCat(CatType type) {
      var rank = GetRankForCat(type);

      // TODO: hotfix, change later
      var convo = GetInfoForCat(type).rankConversations;
      if(rank >= convo.Count){
        rank = convo.Count - 1;
      }

      return convo[rank].dialogue;
    }

    public void UpdateRelationshipState() {
      catRanks = new Dictionary<CatType, int>();
      foreach (var catInfo in catInfos) {
        //TODO(dwong): add rank based on saved state.
        catRanks.Add(catInfo.type, 0);
      }
    }
    
    private CatInfo GetInfoForCat(CatType type) {
      return catInfos.FirstOrDefault(cat => cat.type == type);
    }
  }
  
  public enum CatType {
    NONE,
    CROOK,
    BEAUTY,
    JOKER,
    SLIM
  }
  
  [Serializable]
  public class CatInfo {
    public CatType type;
    public List<SerializedDialogue> rankConversations;
  }
  
  // Wrapper class for our conversation list, so it can be serialized.
  [Serializable]
  public class SerializedDialogue {
    public TextAsset[] dialogue;
  }
}