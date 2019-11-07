using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw{

  [CreateAssetMenu(fileName = "New Object Dialogue Data", menuName = "Outclaw/Dialogue/New Object Data")]
  public class ObjectDialogueData : ScriptableObject
  {
    [System.NonSerialized] private int count = 0;

    public int Progress{
      get => count;
    }

    public void Increment(){
      ++count;
    }

    public void Reset(){
      count = 0;
    }
  }
}
