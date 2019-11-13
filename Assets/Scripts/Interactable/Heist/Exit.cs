using System.Collections;

using Outclaw.City;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Outclaw.Heist {
  public class Exit : MonoBehaviour, Interactable {
    [SerializeField]
    private Indicator exitIndicator;

    [SerializeField]
    private LocationData exitScene;

    [SerializeField]
    private AudioClip victorySound;

    [SerializeField]
    private VictoryMenu menu;

    [SerializeField]
    private GameStateData taskToOpen;

    [Inject]
    private ISoundManager soundManager;

    [Inject]
    private ISceneTransitionManager transition;

    #if UNITY_EDITOR
    void Awake(){
      if(exitScene == null){
        Debug.LogError("Exit Scene not set in " + gameObject);
      }
      if(taskToOpen == null){
        Debug.LogError("Required task not set in " + gameObject);
      }
    }
    #endif

    public void InRange() {
      exitIndicator.FadeIn();
    }

    public void ExitRange() {
      exitIndicator.FadeOut();
    }

    public void Interact() {
      if (!taskToOpen.HasCompleteObjective) {
        return;
      }
      if(victorySound == null){
        transition.TransitionToScene(exitScene);
      }
      else{
        StartCoroutine(StartExit());
      }
    }

    private IEnumerator StartExit(){
      soundManager.PlaySFX(victorySound);
      menu.Show();
      yield return new WaitForSeconds(victorySound.length);
      transition.TransitionToScene(exitScene);
    }
  }
}