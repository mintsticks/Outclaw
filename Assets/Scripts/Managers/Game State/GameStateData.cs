using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Outclaw{
  [CreateAssetMenu(fileName = "Game State Data", menuName = "Outclaw/Game State Data")]
  public class GameStateData : ScriptableObject
  {
    public List<GameStateObjectives> childStates;
  }
}
