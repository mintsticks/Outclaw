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
    private string exitScene = "Main";

    [SerializeField]
    private AudioClip victorySound;

    [SerializeField]
    private VictoryMenu menu;

    [Inject]
    private IObjectiveManager objectiveManager;

    [Inject]
    private ISoundManager soundManager;
    
    private bool isComplete;
    public void InRange() {
      exitIndicator.FadeIn();
    }

    public void ExitRange() {
      exitIndicator.FadeOut();
    }

    public void Interact() {
      if (!objectiveManager.ObjectivesComplete()) {
        return;
      }
      if(victorySound == null){
        SceneManager.LoadScene(exitScene);
      }
      else{
        StartCoroutine(StartExit());
      }
    }

    private IEnumerator StartExit(){
      soundManager.PlaySFX(victorySound);
      menu.Show();
      yield return new WaitForSeconds(victorySound.length);
      SceneManager.LoadScene(exitScene);
    }
  }
}