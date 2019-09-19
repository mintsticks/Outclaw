using UnityEngine;
using Zenject;

namespace Outclaw {
  public class GameInstaller : MonoInstaller {
    [SerializeField]
    private GameObject soundManagerPrefab;
    
    /// <summary>
    /// For all classes that are common to all scenes.
    /// Bind the interfaces to the concrete classes.
    /// </summary>
    public override void InstallBindings() {
      Container.Bind<IPlayerData>()
        .To<PlayerData>()
        .AsSingle();
      Container.Bind<IPlayerInput>()
        .To<PlayerInput>()
        .AsSingle();
      Container.Bind<ISoundManager>()
        .To<SoundManager>()
        .FromComponentInNewPrefab(soundManagerPrefab)
        .AsSingle();
    }
  }
}