using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Outclaw.UI;
using Utility;

namespace Outclaw {
  public class ControlDisplay : MonoBehaviour {

    [SerializeField] private PlatformInfo info;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Text buttonText;
    [SerializeField] private Text actionText;
    [SerializeField] private InputType inputType;

    void Awake() {
      buttonImage.sprite = info.images.FirstOrDefault(i => i.platform == Application.platform)?.image;
      buttonText.text = InputStringHelper.GetStringForInput(inputType);
      actionText.text = inputType.ToString();
    }

  }
}