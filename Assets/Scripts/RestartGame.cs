using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Outclaw{
  public interface IRestartGame{
    void Restart();
  }

  public class RestartGame : MonoBehaviour, IRestartGame
  {

    [SerializeField] private LocationData startScene;

    [Inject] private ISceneTransitionManager sceneTransitionManager;
    [Inject] private IGameStateManager gameStateManager;

    public void Restart(){

      sceneTransitionManager.TransitionToScene(startScene);
      gameStateManager.SetGameState(gameStateManager.StateList[0], true);
    }
  }
}
