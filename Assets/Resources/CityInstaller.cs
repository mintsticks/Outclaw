#pragma warning disable 649

using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public class CityInstaller : MonoInstaller {
    [SerializeField]
    private Player playerInstance;

    [SerializeField] 
    private GameObject pauseMenuManagerPrefab;
    
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
    }
    
    private void BindFactories() {
      Container.BindFactory<PromptType,
          IDismissablePrompt, 
          DismissablePromptFactory>()
        .FromFactory<CustomPromptFactory>();
    }
  }
}