using City;
using Outclaw.City;
using UnityEngine;
using Zenject;

namespace Outclaw {
  public class GameInstaller : MonoInstaller {
    [SerializeField] private GameObject soundManagerPrefab;
    [SerializeField] private GameObject sceneTransitionManagerPrefab;
    [SerializeField] private GameObject objectiveManagerPrefab;
    [SerializeField] private PauseGame pause;
    [SerializeField] private GameObject spawnManagerPrefab;
    [SerializeField] private GameObject promptDisplayPrefab;

    /// <summary>
    /// For all classes that are common to all scenes.
    /// Bind the interfaces to the concrete classes.
    /// </summary>
    public override void InstallBindings() {
      Container.Bind<IPlayerData>()
        .To<PlayerData>()
        .AsSingle();
      Container.BindInterfacesAndSelfTo<PlayerInput>()
        .AsSingle();
      Container.Bind<IPauseGame>()
        .FromInstance(pause)
        .AsSingle();

      InstallManagers();
      InstallFactories();
    }

    private void InstallManagers() {
      Container.Bind<ISoundManager>()
        .To<SoundManager>()
        .FromComponentInNewPrefab(soundManagerPrefab)
        .AsSingle();
      Container.Bind<ISceneTransitionManager>()
        .To<SceneTransitionManager>()
        .FromComponentInNewPrefab(sceneTransitionManagerPrefab)
        .AsSingle();
      Container.Bind<IObjectiveManager>()
        .To<ObjectiveManager>()
        .FromComponentInNewPrefab(objectiveManagerPrefab)
        .AsSingle()
        .NonLazy();
      Container.Bind<ISpawnManager>()
        .To<SpawnManager>()
        .FromComponentInNewPrefab(spawnManagerPrefab)
        .AsSingle();
      Container.BindFactory<PromptDisplay.Data, 
          PromptDisplay, 
          PromptDisplay.Factory>()
        .FromComponentInNewPrefab(promptDisplayPrefab);
      Container.BindInterfacesAndSelfTo<RelationshipManager>()
        .AsSingle();
      Container.BindInterfacesAndSelfTo<LocationManager>()
        .AsSingle();
      Container.BindInterfacesAndSelfTo<GameStateManager>()
        .AsSingle();
    }

    private void InstallFactories() { }
  }
}