using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Outclaw.City {
  public class InteractableCat : MonoBehaviour, Interactable {
    [SerializeField]
    private Indicator talkIndicator;

    [SerializeField]
    private CatType type;
    
    [Inject]
    private IPlayer player;

    [Inject]
    private IRelationshipManager relationshipManager;

    [Inject]
    private IDialogueManager dialogueManager;
    
    private Transform parent;
    private bool created;
    
    public void Awake() {
      talkIndicator.Initialize(player.PlayerTransform);
    }

    public void InRange() {
      talkIndicator.CreateIndicator();
      StartCoroutine(talkIndicator.FadeIn());
    }

    public void ExitRange() {
      StartCoroutine(talkIndicator.FadeOut());
    }

    public void Interact() {
      StartCoroutine(talkIndicator.FadeOut());
      var dialogue = relationshipManager.GetDialogueForCat(type);
      dialogueManager.SetDialogueType(DialogueType.SPEECH);
      dialogueManager.SetDialogue(dialogue);
      dialogueManager.SetBubbleParent(transform);
      dialogueManager.StartDialogue(CompleteInteraction);
    }
    
    private void CompleteInteraction() {
      relationshipManager.RankUpCat(type);
      InRange();
    }
  }
}