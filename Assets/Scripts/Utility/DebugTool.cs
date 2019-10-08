using UnityEngine;
using Zenject;

namespace Outclaw {
  public class DebugTool : MonoBehaviour {
    [SerializeField]
    private GameStateType debugGameState;

    [Inject]
    private IGameStateManager gameStateManager;

    [ContextMenu("SetGameState")]
    private void SetGameState() {
      gameStateManager.SetGameState(debugGameState, true);
    }

    [ContextMenu("PrintGameState")]
    private void PrintGameState() {
      Debug.Log(gameStateManager.CurrentGameState);
    }
  }
}