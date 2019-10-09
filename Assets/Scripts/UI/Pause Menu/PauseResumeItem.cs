using UnityEngine;
using Zenject;
using Outclaw.UI;

namespace Outclaw {
  public class PauseResumeItem : MonoBehaviour, IMenuItem {
    [SerializeField]
    private MenuItemText pauseItemText;
    
    [Inject]
    private IPauseMenuManager pauseMenuManager;
    
    public void Select() {
      pauseMenuManager.Unpause();
    }

    public void Hover() {
      pauseItemText.Hover();
    }

    public void Unhover() {
      pauseItemText.Unhover();
    }
  }
}