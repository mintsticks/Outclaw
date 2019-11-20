using UnityEngine;
using Zenject;
using Outclaw.UI;

namespace Outclaw {
  public class SubmenuExitItem : MonoBehaviour, IMenuItem {
    
    [SerializeField]
    private MenuItemText menuItemText;
    
    public void Select() {
      throw new System.NotImplementedException();
      //make submenu inactive
      //
    }

    public void Hover() {
      menuItemText.Hover();
    }

    public void Unhover() {
      menuItemText.Hover();
    }
  }
}