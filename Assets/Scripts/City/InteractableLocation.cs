using System.Collections;
using UnityEngine;

namespace Outclaw.City {
  public class InteractableLocation : MonoBehaviour, Interactable {
    [SerializeField]
    private Indicator enterIndicator;
    public void Awake() {
      enterIndicator.Initialize(GameManager.Instance.PlayerInstance.transform);
    }
    
    public void InRange() {
      enterIndicator.CreateIndicator();
      StartCoroutine(enterIndicator.FadeIn());
    }

    public void ExitRange() {
      StartCoroutine(enterIndicator.FadeOut());
    }

    public void Interact() {
     
    }
  }
}