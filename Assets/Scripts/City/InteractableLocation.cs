using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Outclaw.City {
  public class InteractableLocation : MonoBehaviour, Interactable {
    [SerializeField]
    private Indicator enterIndicator;

    [SerializeField]
    private string locationName;

    [Inject]
    private IPlayer player;
    
    public void Awake() {
      enterIndicator.Initialize(player.PlayerTransform);
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