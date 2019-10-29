using UnityEngine;
using Zenject;
using Outclaw.UI;

namespace Outclaw {
  public class PauseExitItem : MonoBehaviour, IMenuItem {
    [SerializeField]
    private MenuItemText pauseItemText;

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
      gameStateManager.SetGameState(gameStateManager.StateList[0], true);
    }

    public void Hover() {
      pauseItemText.Hover();
    }

    public void Unhover() {
      pauseItemText.Unhover();
    }
  }
}