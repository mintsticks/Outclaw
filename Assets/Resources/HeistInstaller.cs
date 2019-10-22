using Managers;
using Outclaw.City;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public class HeistInstaller : MonoInstaller {
    [SerializeField]
    private GameObject abilityManagerPrefab;

    [SerializeField]
    private PlayerController controller;

    [SerializeField]
    private GameObject capturedMenuPrefab;

    [SerializeField] private GameObject footprintPrefab;
    
    /// <summary>
    /// For all classes common to heist scenes.
    /// Bind the interfaces to the concrete classes.
    /// </summary>
    public override void InstallBindings() {
      Container.Bind<IAbilityCooldownManager>()
        .To<AbilityCooldownManager>()
        .FromComponentInNewPrefab(abilityManagerPrefab)
        .AsSingle()
        .NonLazy();
      Container.Bind<ICapturedMenu>()
        .To<CapturedMenu>()
        .FromComponentInNewPrefab(capturedMenuPrefab)
        .AsSingle()
        .NonLazy();
      Container.Bind<IObjectiveManager>()
        .To<ObjectiveManager>()
        .AsSingle()
        .NonLazy();
      Container.Bind<IPlayer>()
        .FromInstance(controller)
        .AsSingle()
        .NonLazy();
      Container.Bind<IHideablePlayer>()
        .FromInstance(controller)
        .AsSingle()
        .NonLazy();
      Container.BindInterfacesAndSelfTo<SneakManager>()
        .AsSingle()
        .NonLazy();
      
      BindFactories();
    }
    
    private void BindFactories() {
      Container.BindFactory<Footprint.Data, 
          Footprint, 
          Footprint.Factory>()
        .FromComponentInNewPrefab(footprintPrefab);
    }
  }
}