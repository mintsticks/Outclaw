using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Outclaw{
  public class SceneSwitcher : MonoBehaviour
  {
    [Inject] private ISceneTransitionManager transition;

    public void SwitchToScene(string scene){
      transition.TransitionToScene(scene);
    }

    public void SwitchToLocation(LocationData location){
      transition.TransitionToScene(location);
    }

    public void ReloadCurrent(){
      transition.TransitionToScene(SceneManager.GetActiveScene().name);
    }
  }
}
