using Zenject;

namespace Outclaw {
  public class GameInstaller : MonoInstaller {
    /// <summary>
    /// For all classes that are common to all scenes.
    /// Bind the interfaces to the concrete classes.
    /// </summary>
    public override void InstallBindings() {
      Container.Bind<IPlayerData>()
        .To<PlayerData>()
        .AsSingle();
    }
  }
}