using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Outclaw{
  public class PauseInstaller : MonoInstaller
  {

    [SerializeField] 
    private GameObject pauseMenuManagerPrefab;

    public override void InstallBindings() {
      Container.Bind<IPauseMenuManager>()
        .To<PauseMenuManager>()
        .FromComponentInNewPrefab(pauseMenuManagerPrefab)
        .AsSingle()
        .NonLazy();
    }
  }
}
