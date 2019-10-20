using System;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using System.Linq;
using City;
using Zenject;

namespace Outclaw.City {
  public class InteractableCat : MonoBehaviour, CityInteractable {
    [SerializeField]
    private CatDialogues catDialogues;
    
    [SerializeField]
    private Indicator talkIndicator;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Transform catPosition;
    
    [SerializeField]
    private ParticleSystem particleSystem;
    
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
    
    [Inject]
    private ISenseManager senseManager;
    
    private Transform parent;
    private bool created;

    public CatType CatType => type;
    public Transform CatPosition => catPosition != null ? catPosition : transform;
    
    public void Awake() {
      talkIndicator.Initialize(player.PlayerTransform);
      objectiveTransformManager.Cats.Add(this);
      senseManager.RegisterCityInteractable(this);
    }

    public void InRange() {
      if (!HasInteraction()) {
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

    public bool HasInteraction() {
      return HasDialogueForCurrentRank() || HasDialogueForCurrentState();
    }

    public void EnableEffect() {
      if (particleSystem == null) {
        return;
      }
      particleSystem.gameObject.SetActive(true);
      particleSystem.Play();
    }

    public void DisableEffect() {
      if (particleSystem == null) {
        return;
      }
      particleSystem.Stop();
      particleSystem.gameObject.SetActive(false);
    }

    public SpriteRenderer GetSpriteRenderer() {
      return spriteRenderer;
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
        //TODO(dwong): add non required game state dialogue
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