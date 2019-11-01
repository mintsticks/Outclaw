using City;
using Managers;
using Outclaw.City;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public class HeistInstaller : MonoInstaller {
    [SerializeField] private GameObject abilityManagerPrefab;
    [SerializeField] private CameraBehavior cameraBehavior;
    [SerializeField] private PlayerController controller;
    [SerializeField] private HeistInteractionController interactionController;
    [SerializeField] private GameObject capturedMenuPrefab;
    [SerializeField] private GameObject footprintPrefab;
    [SerializeField] private GameObject senseManager;
    [SerializeField] private GameObject vantageManagerPrefab;
    [SerializeField] private GameObject sneakManagerPrefab;
    [SerializeField] private GameObject objectiveTransformManagerPrefab;
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
      Container.Bind<IHeistInteractionController>()
        .FromInstance(interactionController)
        .AsSingle()
        .NonLazy();
      Container.Bind<ICameraBehavior>()
        .To<CameraBehavior>()
        .FromInstance(cameraBehavior)
        .AsSingle()
        .NonLazy();
      Container.Bind<IHideablePlayer>()
        .FromInstance(controller)
        .AsSingle()
        .NonLazy();
      Container.Bind<ISneakManager>()
        .To<SneakManager>()
        .FromComponentInNewPrefab(sneakManagerPrefab)
        .AsSingle()
        .NonLazy();
      Container.BindInterfacesAndSelfTo<PlayerLitManager>()
        .AsSingle()
        .NonLazy();
      Container.Bind<ISenseManager>()
        .To<SenseManager>()
        .FromComponentInNewPrefab(senseManager)
        .AsSingle()
        .NonLazy();
      Container.Bind<IObjectiveTransformManager>()
        .To<ObjectiveTransformManager>()
        .FromComponentInNewPrefab(objectiveTransformManagerPrefab)
        .AsSingle()
        .NonLazy();
      Container.Bind<IVantagePointManager>()
        .To<VantagePointManager>()
        .FromComponentInNewPrefab(vantageManagerPrefab)
        .AsSingle()
        .NonLazy();
      Container.Bind<ISenseVisuals>()
        .To<HeistSenseVisuals>()
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