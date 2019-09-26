using UnityEngine;

namespace Outclaw.City {
  public class PauseOptionItem : MonoBehaviour, IPauseItem {
    [SerializeField]
    private PauseItemText pauseItemText;
    
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