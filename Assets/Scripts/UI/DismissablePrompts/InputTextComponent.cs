using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI.DismissablePrompts {
  public class InputTextComponent : MonoBehaviour {
    [SerializeField] private Text component;
    [SerializeField] private InputType inputType;
    
    private void Start() {
      component.text = InputStringHelper.GetStringForInput(inputType);
    }
  }
}