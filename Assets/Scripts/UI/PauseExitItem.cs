using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public class PauseExitItem : MonoBehaviour, IPauseItem {
    [SerializeField]
    private PauseItemText pauseItemText;

    [Inject]
    private ISceneTransitionManager sceneTransitionManager;

    [Inject]
    private IPauseMenuManager pauseMenuManager;

    [Inject]
    private IGameStateManager gameStateManager;
    
    public void Select() {
      pauseMenuManager.Unpause();
      sceneTransitionManager.TransitionToScene("Start");

      // TODO: replace with actual restart
      gameStateManager.SetGameState(GameStateType.TUTORIAL, true);
    }

    public void Hover() {
      pauseItemText.Hover();
    }

    public void Unhover() {
      pauseItemText.Unhover();
    }
  }
}