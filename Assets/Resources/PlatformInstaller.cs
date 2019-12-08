using UI.DismissablePrompts;
using Zenject;

namespace Outclaw {
  public class PlatformInstaller : MonoInstaller {
    public override void InstallBindings() {
      BindFactories();
    }

    private void BindFactories() {
      Container.BindFactory<PlatformDependentComponent,
          IPlatformDependentCanvasComponent, 
          PlatformDependentCanvasFactory>()
        .FromFactory<CustomCanvasFactory>();
    }
  }
}