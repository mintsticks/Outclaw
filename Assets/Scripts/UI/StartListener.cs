using System.Collections;
using Outclaw;
using Outclaw.City;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using Zenject;

public class StartListener : MonoBehaviour {

  [SerializeField]
  private AudioClip startSound;

  [SerializeField] 
  private Text startText;

  [Inject]
  private IPlayerInput playerInput;

  [Inject]
  private ISceneTransitionManager sceneTransitionManager;
  
  [Inject]
  private ISoundManager soundManager;
  
  void Awake() {
    startText.text = "press " + InputStringHelper.GetStringForInput(InputType.START);
  }
  
  void Update() {
    
    if (!playerInput.IsStartDown()) {
      return;
    }
    
    soundManager.PlaySFX(startSound);
    LoadIntro();
  }
  
  private void LoadIntro() {
    sceneTransitionManager.TransitionToScene("Intro");
  }
}