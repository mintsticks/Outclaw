using System.Collections;
using Outclaw;
using Outclaw.City;
using UnityEngine;
using Zenject;

public class StartListener : MonoBehaviour {

  [SerializeField]
  private AudioClip startSound;

  [Inject]
  private IPlayerInput playerInput;

  [Inject]
  private ISceneTransitionManager sceneTransitionManager;
  
  [Inject]
  private ISoundManager soundManager;

  void Update() {
    if (!playerInput.IsInteractDown()) {
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