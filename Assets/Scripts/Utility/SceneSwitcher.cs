using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Outclaw{
  public class SceneSwitcher : MonoBehaviour
  {
    [Inject] private ISceneTransitionManager tranition;

    public void SwitchToScene(string scene){
      tranition.TransitionToScene(scene);
    }

    public void ReloadCurrent(){
      tranition.TransitionToScene(SceneManager.GetActiveScene().name);
    }
  }
}
