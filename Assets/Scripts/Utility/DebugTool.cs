using UnityEngine;
using Zenject;
using City;

namespace Outclaw {
  public class DebugTool : MonoBehaviour {
    [SerializeField]
    private GameStateData debugGameState;

    [Inject]
    private IGameStateManager gameStateManager;

    [Inject]
    private IObjectiveManager objectiveManager;

    [ContextMenu("SetGameState")]
    private void SetGameState() {
      gameStateManager.SetGameState(debugGameState, true);
    }

    [ContextMenu("PrintGameState")]
    private void PrintGameState() {
      Debug.Log(gameStateManager.CurrentGameStateData);
    }
  }
}