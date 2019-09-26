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
    
    public void Select() {
      pauseMenuManager.Unpause();
      sceneTransitionManager.TransitionToScene("Start");
    }

    public void Hover() {
      pauseItemText.Hover();
    }

    public void Unhover() {
      pauseItemText.Unhover();
    }
  }
}