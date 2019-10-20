using UnityEngine;
using Outclaw.UI;

namespace Outclaw {
  public class PauseSaveItem : MonoBehaviour, IMenuItem {
    [SerializeField]
    private MenuItemText pauseItemText;
    
    public void Select() {
      throw new System.NotImplementedException();
    }

    public void Hover() {
      pauseItemText.Hover();
    }

    public void Unhover() {
      pauseItemText.Unhover();
    }
  }
}