#pragma warning disable 649

using City;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public class CityInstaller : MonoInstaller {
    [SerializeField] private Player playerInstance;
    [SerializeField] private CameraBehavior cameraInstance;
    [SerializeField] private GameObject objectiveTransformManagerPrefab;
    [SerializeField] private GameObject senseManagerPrefab;
    [SerializeField] private GameObject cityDialogueSettingPrefab;

    /// <summary>
    /// For all classes common to city scenes.
    /// Bind the interfaces to the concrete classes.
    /// </summary>
    public override void InstallBindings() {
      BindComponents();
    }

    private void BindComponents() {
      Container.Bind<IPlayer>()
        .To<Player>()
        .FromInstance(playerInstance)
        .AsSingle()
        .NonLazy();
      Container.Bind<ICameraBehavior>()
        .To<CameraBehavior>()
        .FromInstance(cameraInstance)
        .AsSingle()
        .NonLazy();
      Container.Bind<IObjectiveTransformManager>()
        .To<ObjectiveTransformManager>()
        .FromComponentInNewPrefab(objectiveTransformManagerPrefab)
        .AsSingle()
        .NonLazy();
      Container.Bind<ISenseManager>()
        .To<SenseManager>()
        .FromComponentInNewPrefab(senseManagerPrefab)
        .AsSingle()
        .NonLazy();
      Container.Bind<ISenseVisuals>()
        .To<CitySenseVisuals>()
        .AsSingle()
        .NonLazy();
      Container.Bind<IDialogueSettings>()
        .To<DialogueSettings>()
        .FromComponentInNewPrefab(cityDialogueSettingPrefab)
        .AsSingle();
    }
  }
}