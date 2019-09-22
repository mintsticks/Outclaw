using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Outclaw.City {
  public class OptionIndicator : MonoBehaviour {
    public class Factory : PlaceholderFactory<OptionIndicator> { }
    
    [SerializeField]
    private Image indicator;

    [SerializeField]
    private Color defaultColor;

    [SerializeField]
    private Color selectedColor;
    
    public void Select() {
      indicator.color = selectedColor;
    }

    public void Deselect() {
      indicator.color = defaultColor;
    }
  }
}