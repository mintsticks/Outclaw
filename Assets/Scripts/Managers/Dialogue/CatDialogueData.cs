using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw.City{
  [CreateAssetMenu(fileName = "New Cat Dialogue", menuName = "Outclaw/Dialogue/New Cat Dialogue")]
  public class CatDialogueData : ScriptableObject
  {
    [System.NonSerialized] private int rank = 0;
    [System.NonSerialized] private int stateRank = 0;

    public int Rank { get => rank; }
    public int GameStateRank { get => stateRank; }

    public void IncreaseRank(){
      ++rank;
    }

    public void IncreateGameStateRank(){
      ++stateRank;
    }

    public void Reset(){
      ResetRank();
      ResetStateRank();
    }

    public void ResetRank(){
      rank = 0;
    }

    public void ResetStateRank(){
      stateRank = 0;
    }
  }
}
