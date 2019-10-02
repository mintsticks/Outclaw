using City;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Outclaw.City {
  public class InteractableLocation : MonoBehaviour, Interactable {
    [SerializeField]
    private Indicator enterIndicator;

    [SerializeField]
    private string locationName;

    [SerializeField]
    private AudioClip enterClip;

    [Inject]
    private IPlayer player;

    [Inject]
    private ISoundManager soundManager;

    [Inject]
    private IObjectiveManager objectiveManager;
    
    [Inject]
    private ISceneTransitionManager sceneTransitionManager;
    
    [Inject]
    private IGameStateManager gameStateManager;
    
    public void Awake() {
      enterIndicator.Initialize(player.PlayerTransform);
    }
    
    public void InRange() {
      if (!objectiveManager.GameStateObjectivesComplete()) {
        return;
      }
      enterIndicator.CreateIndicator();
      StartCoroutine(enterIndicator.FadeIn());
    }

    public void ExitRange() {
      StartCoroutine(enterIndicator.FadeOut());
    }

    public void Interact() {
      if (!objectiveManager.GameStateObjectivesComplete()) {
        return;
      }
      
      if(enterClip != null){
        soundManager.PlaySFX(enterClip);
      }
      

      gameStateManager.CurrentGameState = GameState.CITY;
      sceneTransitionManager.TransitionToScene(locationName);
    }
  }
}