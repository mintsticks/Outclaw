#pragma warning disable 649

using UnityEngine;
using Zenject;

namespace Outclaw {
  public class PromptInstaller : MonoInstaller {
    [SerializeField]
    private PromptSettings promptSettings;
    
    public override void InstallBindings() {
      Container.BindInstance(promptSettings);
      BindFactories();
    }
    
    private void BindFactories() {
      Container.BindFactory<PromptType,
          IDismissablePrompt, 
          DismissablePromptFactory>()
        .FromFactory<CustomPromptFactory>();
    }
  }
}