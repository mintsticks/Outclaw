using Outclaw.Heist;
using UnityEngine;
using Zenject;

namespace Outclaw {
  public class CheckpointUpdater : MonoBehaviour {

    [Inject] private City.IPlayer player;
    [Inject] private ISpawnManager spawnManager;
    [Inject] private IPauseGame pauseGame;

    public void UpdateCheckpoint() {
      var point = spawnManager.GetCheckpoint();
      player.UpdatePosition(point);
      player.InputDisabled = false;
    }

  }
}