using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public class CityInstaller : MonoInstaller {
    [SerializeField]
    private GameObject playerPrefab;
    
    /// <summary>
    /// For all classes common to city scenes.
    /// Bind the interfaces to the concrete classes.
    /// </summary>
    public override void InstallBindings() {
      Container.Bind<IPlayer>()
        .To<Player>()
        .FromComponentInNewPrefab(playerPrefab)
        .AsSingle()
        .NonLazy();
    }
  }
}