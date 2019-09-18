using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Outclaw.City {
  public class InteractableCat : MonoBehaviour, Interactable {
    [SerializeField]
    private Indicator talkIndicator;

    [SerializeField]
    private string nextScene = "Jail";
   
    [Inject]
    private IPlayer player;
    
    private Transform parent;

    public void Awake() {
      talkIndicator.Initialize(player.PlayerTransform);
    }

    public void InRange() {
      talkIndicator.CreateIndicator();
      StartCoroutine(talkIndicator.FadeIn());
    }

    public void ExitRange() {
      StartCoroutine(talkIndicator.FadeOut());
    }

    public void Interact() {
      //TODO(dwong): Later replace this with hook into minigame or cat socialization
      SceneManager.LoadScene(nextScene);
    }
  }
}