using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw{

  [CreateAssetMenu(fileName = "New Task", menuName = "Outclaw/Task")]
  public class Task : ScriptableObject
  {
    private HashSet<GameStateData> completedIn = new HashSet<GameStateData>();

    public void Complete(GameStateData state){
      completedIn.Add(state);
    }

    public bool IsComplete(GameStateData state){
      return completedIn.Contains(state);
    }
  }
}
