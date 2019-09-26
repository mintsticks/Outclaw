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

    LoadMain();
  }

  private void LoadMain() {
    sceneTransitionManager.TransitionToScene("Main");
  }
}