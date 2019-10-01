using System.Collections;
using Outclaw;
using Outclaw.City;
using UnityEngine;
using Zenject;

public class StartListener : MonoBehaviour {
  [Inject]
  private IPlayerInput playerInput;

  [Inject]
  private ISceneTransitionManager sceneTransitionManager;
  
  void Update() {
    if (!playerInput.IsInteractDown()) {
      return;
    }

    LoadIntro();
  }

  private void LoadMain() {
    sceneTransitionManager.TransitionToScene("Main");
  }

  private void LoadIntro() {
    sceneTransitionManager.TransitionToScene("Intro");
  }
}