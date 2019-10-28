using Outclaw.Heist;
using UnityEngine;
using Zenject;

namespace Outclaw {
  public class CheckpointUpdater : MonoBehaviour {

    [SerializeField] private PlayerController playerController;

    [Inject] private ISpawnManager spawnManager;
    [Inject] private IPauseGame pauseGame;

    public void UpdateCheckpoint() {
      var point = spawnManager.GetCheckpoint();
      playerController.UpdatePosition(point);
      pauseGame.Unpause();
    }

  }
}