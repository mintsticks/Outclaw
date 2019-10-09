#pragma warning disable 649

using City;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public class CityInstaller : MonoInstaller {
    [SerializeField]
    private Player playerInstance;

    [SerializeField] 
    private GameObject pauseMenuManagerPrefab;

    [SerializeField]
    private GameObject objectiveManagerPrefab;

    [SerializeField]
    private GameObject objectiveTransformManagerPrefab;

    [SerializeField]
    private PromptSettings promptSettings;

    /// <summary>
    /// For all classes common to city scenes.
    /// Bind the interfaces to the concrete classes.
    /// </summary>
    public override void InstallBindings() {
      Container.BindInstance(promptSettings);
      BindComponents();
      BindFactories();
    }

    private void BindComponents() {
      Container.Bind<IPlayer>()
        .To<Player>()
        .FromInstance(playerInstance)
        .AsSingle()
        .NonLazy();
      Container.Bind<IPauseMenuManager>()
        .To<PauseMenuManager>()
        .FromComponentInNewPrefab(pauseMenuManagerPrefab)
        .AsSingle();
      Container.Bind<IObjectiveManager>()
        .To<ObjectiveManager>()
        .FromComponentInNewPrefab(objectiveManagerPrefab)
        .AsSingle()
        .NonLazy();
      Container.Bind<IObjectiveTransformManager>()
        .To<ObjectiveTransformManager>()
        .FromComponentInNewPrefab(objectiveTransformManagerPrefab)
        .AsSingle()
        .NonLazy();
    }
    
    private void BindFactories() {
      Container.BindFactory<PromptType,
          IDismissablePrompt, 
          DismissablePromptFactory>()
        .FromFactory<CustomPromptFactory>();
    }
  }
}