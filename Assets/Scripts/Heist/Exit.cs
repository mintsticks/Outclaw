using Outclaw.City;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Outclaw.Heist {
  public class Exit : MonoBehaviour, Interactable {
    [SerializeField]
    private Indicator exitIndicator;

    [SerializeField]
    private string exitScene = "Main";
    
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
      //TODO(dwong): check if heist objectives have been completed in injected class
      SceneManager.LoadScene(exitScene);
    }
  }
}