using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw.City{
  [CreateAssetMenu(fileName = "New Cat Dialogue", menuName = "Outclaw/Dialogue/New Cat Dialogue")]
  public class CatDialogueData : ScriptableObject
  {
    [System.NonSerialized] private int rank = 0;
    [System.NonSerialized] private Dictionary<GameStateData, int> stateRanks = new Dictionary<GameStateData, int>();

    public int Rank { get => rank; }
    public int GetGameStateRank(GameStateData state) {
      if(!stateRanks.ContainsKey(state)){
        return 0;
      }
      return stateRanks[state]; 
    }

    public void IncreaseRank(){
      ++rank;
    }

    public void IncreaseGameStateRank(GameStateData state){
      CheckForGameState(state);
      ++stateRanks[state];
    }

    private void CheckForGameState(GameStateData state){
      if(!stateRanks.ContainsKey(state)){
        stateRanks.Add(state, 0);
      }
    }

    public void Reset(){
      ResetRank();
      ResetStateRanks();
    }

    public void ResetRank(){
      rank = 0;
    }

    public void ResetStateRank(GameStateData state){
      CheckForGameState(state);
      stateRanks[state] = 0;
    }

    public void ResetStateRanks(){
      stateRanks.Clear();
    }
  }
}
