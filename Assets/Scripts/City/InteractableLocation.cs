using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Outclaw.City {
  public class InteractableLocation : MonoBehaviour, Interactable {
    [SerializeField]
    private Indicator enterIndicator;

    [SerializeField]
    private string locationName;
    
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
      SceneManager.LoadScene(locationName);
    }
  }
}