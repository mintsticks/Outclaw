using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public class CityInstaller : MonoInstaller {
    [SerializeField]
    private Player playerInstance;

    [SerializeField]
    private GameObject dialogueManagerPrefab;

    [SerializeField]
    private GameObject speechBubblePrefab;

    [SerializeField]
    private GameObject thoughtBubblePrefab;

    [SerializeField]
    private GameObject optionIndicatorPrefab;

    [SerializeField]
    private GameObject dialogueIconManagerPrefab;

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
      Container.Bind<IDialogueManager>()
        .To<DialogueManager>()
        .FromComponentInNewPrefab(dialogueManagerPrefab)
        .AsSingle()
        .NonLazy();
      Container.Bind<IDialogueIconManager>()
        .To<DialogueIconManager>()
        .FromComponentInNewPrefab(dialogueIconManagerPrefab)
        .AsSingle();
      Container.Bind<IPauseMenuManager>()
        .To<PauseMenuManager>()
        .FromComponentInNewPrefab(pauseMenuManagerPrefab)
        .AsSingle();
    }
    
    private void BindFactories() {
      Container.BindFactory<SpeechBubble.Data, 
          SpeechBubble, 
          SpeechBubble.Factory>()
        .FromComponentInNewPrefab(speechBubblePrefab);
      Container.BindFactory<ThoughtBubble.Data, 
          ThoughtBubble, 
          ThoughtBubble.Factory>()
        .FromComponentInNewPrefab(thoughtBubblePrefab);
      Container.BindFactory<OptionIndicator, 
          OptionIndicator.Factory>()
        .FromComponentInNewPrefab(optionIndicatorPrefab);
      Container.BindFactory<PromptType,
          IDismissablePrompt, 
          DismissablePromptFactory>()
        .FromFactory<CustomPromptFactory>();
    }
  }
}