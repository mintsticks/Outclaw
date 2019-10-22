using City;
using Outclaw.City;
using UnityEngine;
using Zenject;

namespace Outclaw {
  public class GameInstaller : MonoInstaller {
    [SerializeField]
    private GameObject soundManagerPrefab;

    [SerializeField]
    private GameObject sceneTransitionManagerPrefab;

    [SerializeField]
    private GameObject objectiveManagerPrefab;
    
    [SerializeField]
    private PauseGame pause;

    /// <summary>
    /// For all classes that are common to all scenes.
    /// Bind the interfaces to the concrete classes.
    /// </summary>
    public override void InstallBindings() {
      Container.Bind<IPlayerData>()
        .To<PlayerData>()
        .AsSingle();
      Container.Bind<IPlayerInput>()
        .To<KeyboardInput>()
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
      Container.BindInterfacesAndSelfTo<RelationshipManager>()
        .AsSingle();
      Container.BindInterfacesAndSelfTo<LocationManager>()
        .AsSingle();
      Container.BindInterfacesAndSelfTo<GameStateManager>()
        .AsSingle();
    }
    
    private void InstallFactories() {
    }
  }
}