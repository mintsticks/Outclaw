using UnityEngine;
using UnityEngine.UI;

namespace Outclaw.UI {
  public class MenuItemText : MonoBehaviour {
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

    public void Hold(){
      itemText.color = hoverColor;
      itemText.fontSize = defaultSize;
    }
  }
}