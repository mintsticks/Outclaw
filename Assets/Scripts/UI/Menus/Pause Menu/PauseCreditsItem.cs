using UnityEngine;
using Outclaw.UI;

namespace Outclaw {
  public class PauseCreditsItem : MonoBehaviour, IMenuItem {
    
    [SerializeField]
    private MenuItemText pauseItemText;
    
    public void Select() {
      throw new System.NotImplementedException();
      //Open new menu ting
      //Disable input on main menu
    }

    public void Hover() {
      pauseItemText.Hover();
    }

    public void Unhover() {
      pauseItemText.Unhover();
    }
  }
}