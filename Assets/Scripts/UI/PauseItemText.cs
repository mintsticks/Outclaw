using UnityEngine;
using UnityEngine.UI;

namespace Outclaw.City {
  public class PauseItemText : MonoBehaviour {
    [SerializeField]
    private Text itemText;

    [SerializeField]
    private Color hoverColor;

    [SerializeField]
    private Color defaultColor;

    [SerializeField]
    private int hoverSize;
    
    [SerializeField]
    private int defaultSize;

    public void Hover() {
      itemText.color = hoverColor;
      itemText.fontSize = hoverSize;
    }

    public void Unhover() {
      itemText.color = defaultColor;
      itemText.fontSize = defaultSize;
    }
  }
}