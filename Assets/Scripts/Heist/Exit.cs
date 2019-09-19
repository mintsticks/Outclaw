using Outclaw.City;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Outclaw.Heist {
  public class Exit : MonoBehaviour, Interactable {
    [SerializeField]
    private Indicator exitIndicator;

    [SerializeField]
    private string exitScene = "Main";

    [Inject]
    private IObjectiveManager objectiveManager;
    
    private bool isComplete;

    public void Awake() {
      exitIndicator.Initialize(transform);
    }
    
    public void InRange() {
      exitIndicator.CreateIndicator();
      StartCoroutine(exitIndicator.FadeIn());
    }

    public void ExitRange() {
      StartCoroutine(exitIndicator.FadeOut());
    }

    public void Interact() {
      if (!objectiveManager.ObjectivesComplete()) {
        return;
      }
      SceneManager.LoadScene(exitScene);
    }
  }
}