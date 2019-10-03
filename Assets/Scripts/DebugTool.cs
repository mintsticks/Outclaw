using UnityEngine;
using Zenject;

namespace Outclaw {
  public class DebugTool : MonoBehaviour {
    [SerializeField]
    private GameState debugGameState;

    [Inject]
    private IGameStateManager gameStateManager;

    [ContextMenu("SetGameState")]
    private void SetGameState() {
      gameStateManager.CurrentGameState = debugGameState;
    }
  }
}