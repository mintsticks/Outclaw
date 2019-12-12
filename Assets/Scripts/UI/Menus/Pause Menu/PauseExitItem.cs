using UnityEngine;
using Zenject;
using Outclaw.UI;

namespace Outclaw {
  public class PauseExitItem : AbstractMouseMenuItem {

    [SerializeField] private RestartGame restart;

    [Inject]
    private ISceneTransitionManager sceneTransitionManager;

    [Inject]
    private IPauseMenuManager pauseMenuManager;

    [Inject]
    private IGameStateManager gameStateManager;
    
    public override void Select() {
      pauseMenuManager.Unpause();
      restart.Restart();
    }
  }
}