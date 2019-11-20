using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Outclaw{
  [CreateAssetMenu(fileName = "Game State Data", menuName = "Outclaw/Game State Data")]
  public class GameStateData : ScriptableObject {
    [SerializeField] private string objectiveText;
    public string ObjectiveText => objectiveText;

    public List<GameStateObjectives> childStates;

    public bool HasCompleteObjective{
      get => childStates.Any(childState => childState.HasAllTasksComplete);
    }
  }
}
