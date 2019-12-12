using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI.DismissablePrompts {
  public class InputTextComponent : MonoBehaviour {
    [SerializeField] private Text component;
    [SerializeField] private InputType inputType;
    [SerializeField] private string defaultPretext = "";
    [SerializeField] private string defaultPosttext = "";
    
    private void Start() {
      component.text = defaultPretext + InputStringHelper.GetStringForInput(inputType) + defaultPosttext;
    }
  }
}