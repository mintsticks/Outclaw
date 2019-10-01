
using UnityEngine;
using Zenject;

namespace Outclaw.City{
  public class DialogueInstaller : MonoInstaller
  {
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
