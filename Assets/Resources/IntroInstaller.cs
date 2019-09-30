using System.Collections;
using System.Collections.Generic;
using Outclaw.City;
using UnityEngine;
using Zenject;

namespace Outclaw {
  public class IntroInstaller : MonoInstaller {

    [SerializeField] private GameObject playerPrefab;
    
    /// <summary>
    /// For all classes common to city scenes.
    /// Bind the interfaces to the concrete classes.
    /// </summary>
    public override void InstallBindings() {
      BindComponents();
    }

    private void BindComponents()
    {
      Container.Bind<IPlayer>()
        .To<Player>()
        .FromComponentInNewPrefab(playerPrefab)
        .AsSingle()
        .NonLazy();
    }
  }
}
