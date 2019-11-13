using Managers.Dialogue;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public class DialogueInstaller : MonoInstaller {
    [SerializeField] private GameObject dialogueManagerPrefab;
    [SerializeField] private GameObject speechBubblePrefab;
    [SerializeField] private GameObject iconBubblePrefab;
    [SerializeField] private GameObject thoughtBubblePrefab;
    [SerializeField] private GameObject optionIndicatorPrefab;
    [SerializeField] private GameObject dialogueIconManagerPrefab;
    [SerializeField] private GameObject iconNameManagerPrefab;
    
    public override void InstallBindings() {
      BindComponents();
      BindFactories();
    }

    private void BindComponents() {
      Container.Bind<IDialogueManager>()
        .To<DialogueManager>()
        .FromComponentInNewPrefab(dialogueManagerPrefab)
        .AsSingle()
        .NonLazy();
      Container.Bind<IIconNameManager>()
        .To<IconNameManager>()
        .FromComponentInNewPrefab(iconNameManagerPrefab)
        .AsSingle()
        .NonLazy();
      Container.Bind<IDialogueIconManager>()
        .To<DialogueIconManager>()
        .FromComponentInNewPrefab(dialogueIconManagerPrefab)
        .AsSingle();
    }

    private void BindFactories() {
      Container.BindFactory<SpeechBubble.Data,
          SpeechBubble,
          SpeechBubble.Factory>()
        .FromComponentInNewPrefab(speechBubblePrefab);
      Container.BindFactory<IconBubble.Data,
          IconBubble,
          IconBubble.Factory>()
        .FromComponentInNewPrefab(iconBubblePrefab);
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