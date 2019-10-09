using System;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using System.Linq;
using City;
using Zenject;

namespace Outclaw.City {
  public class InteractableCat : MonoBehaviour, Interactable {
    [SerializeField]
    private CatDialogues catDialogues;
    
    [SerializeField]
    private Indicator talkIndicator;

    [SerializeField]
    private CatType type;
    
    [Inject]
    private IPlayer player;

    [Inject]
    private IRelationshipManager relationshipManager;

    [Inject]
    private IGameStateManager gameStateManager;
    
    [Inject]
    private IDialogueManager dialogueManager;
    
    [Inject]
    private ISceneTransitionManager sceneTransitionManager;

    [Inject]
    private IObjectiveManager objectiveManager;

    [Inject] 
    private IObjectiveTransformManager objectiveTransformManager;
    
    private Transform parent;
    private bool created;

    public CatType CatType => type;
    
    public void Awake() {
      talkIndicator.Initialize(player.PlayerTransform);
      objectiveTransformManager.Cats.Add(this);
    }

    public void InRange() {
      if (!HasDialogue()) {
        return;
      }
      talkIndicator.CreateIndicator();
      StartCoroutine(talkIndicator.FadeIn());
    }

    public void ExitRange() {
      StartCoroutine(talkIndicator.FadeOut());
    }

    public void Interact() {
      StartCoroutine(talkIndicator.FadeOut());
      if (HasDialogueForCurrentState()) {
        StartGameStateDialogue(gameStateManager.CurrentGameState);
        return;
      }
      if (HasDialogueForCurrentRank()) {
        StartRelationshipDialogue();
      }
    }
    
    private bool HasDialogue() {
      return HasDialogueForCurrentRank() || HasDialogueForCurrentState();
    }

    private bool HasDialogueForCurrentRank() {
      var rank = relationshipManager.GetRankForCat(type);
      var maxRank = catDialogues.dialoguesForRank.Count - 1;
      return rank <= maxRank;
    }

    private bool HasDialogueForCurrentState() {
      var currentGameState = gameStateManager.CurrentGameState;
      var dialogueForState = GetDialogueForState(currentGameState);

      if (dialogueForState == null) {
        return false;
      }
      
      var maxRank = dialogueForState.catDialogue.Count - 1;
      var rank = relationshipManager.GetRankForCatInGameState(type, currentGameState);
      return rank <= maxRank;
    }
    
    private void StartGameStateDialogue(GameStateType state) {
      var dialogueForState = GetDialogueForState(state);
      var gameStateRank = relationshipManager.GetRankForCatInGameState(type, state);
      var dialogue = dialogueForState.catDialogue[gameStateRank].dialogue;
      
      dialogueManager.SetDialogueType(DialogueType.SPEECH);
      dialogueManager.SetDialogue(dialogue);
      dialogueManager.SetBubbleParent(transform);
      dialogueManager.StartDialogue(() => CompleteGameStateDialogue(state));
    }

    private void CompleteGameStateDialogue(GameStateType state) {
      relationshipManager.RankUpCatInGameState(type, state);
      if (!HasDialogueForCurrentState()) {
        //Completed dialogues for state
        objectiveManager.CompleteConversationObjective(type);
      }
      InRange();
    }
    
    private void StartRelationshipDialogue() {
      var rank = relationshipManager.GetRankForCat(type);
      var dialogue = catDialogues.dialoguesForRank[rank].dialogue;
      
      dialogueManager.SetDialogueType(DialogueType.SPEECH);
      dialogueManager.SetDialogue(dialogue);
      dialogueManager.SetBubbleParent(transform);
      dialogueManager.StartDialogue(CompleteRankDialogue);
    }
    
    private void CompleteRankDialogue() {
      relationshipManager.RankUpCat(type);
      InRange();
    }
    
    private CatDialogueForState GetDialogueForState(GameStateType state) {
      return catDialogues.dialoguesForStates.FirstOrDefault(dialogue => dialogue.gameState == state);
    }

    [YarnCommand("toScene")]
    public void ToScene(string dest) {
      Debug.Log(dest);
      sceneTransitionManager.TransitionToScene(dest);
    }
  }
  
  [Serializable]
  public class CatDialogueForState {
    public GameStateType gameState;
    public List<SerializedDialogue> catDialogue;
  }
  
  [Serializable]
  public class CatDialogues {
    public List<CatDialogueForState> dialoguesForStates;
    public List<SerializedDialogue> dialoguesForRank;
  }
}