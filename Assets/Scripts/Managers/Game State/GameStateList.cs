using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Outclaw{
  [CreateAssetMenu(fileName = "Game State List", menuName = "Outclaw/Game State List")]
  public class GameStateList : ScriptableObject
  {
    [SerializeField] List<GameStateData> states;

    public GameStateData this[int i] => states[i];
  }
}
