using UnityEngine;
using Zenject;
using Outclaw.UI;

namespace Outclaw {
  public class PauseExitItem : AbstractMouseMenuItem {
    [Inject]
    private ISceneTransitionManager sceneTransitionManager;

    [Inject]
    private IPauseMenuManager pauseMenuManager;

    [Inject]
    private IGameStateManager gameStateManager;
    
    public override void Select() {
      pauseMenuManager.Unpause();
      sceneTransitionManager.TransitionToScene("Start");

      // TODO: replace with actual restart
      gameStateManager.SetGameState(gameStateManager.StateList[0], true);
    }
  }
}