using System.Collections;
using Outclaw;
using Outclaw.City;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class StartListener : MonoBehaviour {

  [SerializeField]
  private AudioClip startSound;

  [SerializeField] 
  private Text startText;

  [SerializeField] 
  private string defaultStart;

  [SerializeField] 
  private string xboxStart;

  [Inject]
  private IPlayerInput playerInput;

  [Inject]
  private ISceneTransitionManager sceneTransitionManager;
  
  [Inject]
  private ISoundManager soundManager;


  void Awake() {
#if UNITY_WSA
    startText.text = xboxStart;
#else
    startText.text = defaultStart;
#endif
  }
  
  void Update() {
    
    if (!playerInput.IsStartDown()) {
      return;
    }
    
    soundManager.PlaySFX(startSound);
    LoadIntro();
  }

  private void LoadMain() {
    sceneTransitionManager.TransitionToScene("Main");
  }

  private void LoadIntro() {
    sceneTransitionManager.TransitionToScene("Intro");
  }
}