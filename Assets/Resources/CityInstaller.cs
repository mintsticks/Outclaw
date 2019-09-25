using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public class CityInstaller : MonoInstaller {
    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private GameObject dialogueManagerPrefab;

    [SerializeField]
    private GameObject relationshipManagerPrefab;

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
    
    /// <summary>
    /// For all classes common to city scenes.
    /// Bind the interfaces to the concrete classes.
    /// </summary>
    public override void InstallBindings() {
      BindComponents();
      BindFactories();
    }

    private void BindComponents() {
      Container.Bind<IPlayer>()
        .To<Player>()
        .FromComponentInNewPrefab(playerPrefab)
        .AsSingle()
        .NonLazy();
      Container.Bind<IDialogueManager>()
        .To<DialogueManager>()
        .FromComponentInNewPrefab(dialogueManagerPrefab)
        .AsSingle()
        .NonLazy();
      Container.Bind<IRelationshipManager>()
        .To<RelationshipManager>()
        .FromComponentInNewPrefab(relationshipManagerPrefab)
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
    }
  }
}