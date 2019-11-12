using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw{

  [CreateAssetMenu(fileName = "New Task", menuName = "Outclaw/Task")]
  public class Task : ScriptableObject
  {
    [System.NonSerialized] private bool completed;

    public void Complete(){
      completed = true;
    }

    public bool IsComplete{
      get { return completed; }
    }

    public void Reset(){
      completed = false;
    }
  }
}
