using UnityEngine;
using Zenject;
using Outclaw.UI;

namespace Outclaw {
  public class PauseResumeItem : AbstractMouseMenuItem {
    [SerializeField]
    private MenuItemText pauseItemText;
    
    [Inject]
    private IPauseMenuManager pauseMenuManager;
    
    public override void Select() {
      pauseMenuManager.Unpause();
    }
  }
}