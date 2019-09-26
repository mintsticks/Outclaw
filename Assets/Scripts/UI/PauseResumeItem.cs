using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public class PauseResumeItem : MonoBehaviour, IPauseItem {
    [SerializeField]
    private PauseItemText pauseItemText;
    
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