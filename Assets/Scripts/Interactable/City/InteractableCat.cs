﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using System.Linq;
using City;
using Zenject;

namespace Outclaw.City {
  public class InteractableCat : MonoBehaviour, ObjectiveInteractable, IHaveTask {
    [SerializeField] private CatDialogues catDialogues;
    [SerializeField] private Indicator talkIndicator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D bound;
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private CatDialogueData dialogueData;
    [SerializeField] private Task task;
    
    [Inject] private IPlayer player;
    [Inject] private IRelationshipManager relationshipManager;
    [Inject] private IGameStateManager gameStateManager;
    [Inject] private IDialogueManager dialogueManager;
    [Inject] private ISceneTransitionManager sceneTransitionManager;
    [Inject] private IObjectiveManager objectiveManager;
    [Inject] private IObjectiveTransformManager objectiveTransformManager;
    [Inject] private ISenseVisuals senseVisuals;
    
    private Transform parent;
    private bool created;

    public Task ContainedTask => task;
    public Transform Location => transform;
    public Transform ObjectiveTransform => transform;

    public Bounds ObjectiveBounds => bound == null ? spriteRenderer.bounds : bound.bounds;


    public void Awake() {
      objectiveTransformManager.RegisterTask(this);
      senseVisuals.RegisterSenseElement(this);
    }

    public void InRange() {
      if (!HasInteraction()) {
        return;
      }
      talkIndicator.FadeIn();
    }

    public void ExitRange() {
      talkIndicator.FadeOut();
    }

    public void Interact() {
      talkIndicator.FadeOut();
      if (HasDialogueForCurrentState()) {
        StartGameStateDialogue(gameStateManager.CurrentGameStateData);
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

    private bool HasDialogueForCurrentRank() {
      var rank = dialogueData.Rank;
      var maxRank = catDialogues.dialoguesForRank.Count - 1;
      return rank <= maxRank;
    }

    private bool HasDialogueForCurrentState() {
      var currentGameState = gameStateManager.CurrentGameStateData;
      var dialogueForState = GetDialogueForState(currentGameState);

      if (dialogueForState == null) {
        return false;
      }
      
      var maxRank = dialogueForState.catDialogue.Count - 1;
      var rank = dialogueData.GetGameStateRank(currentGameState);
      return rank <= maxRank;
    }
    
    private void StartGameStateDialogue(GameStateData state) {
      var dialogueForState = GetDialogueForState(state);
      var gameStateRank = dialogueData.GetGameStateRank(state);
      var dialogue = dialogueForState.catDialogue[gameStateRank].dialogue;
      dialogueManager.StartDialogue(dialogue, DialogueType.SPEECH, transform, this, () => CompleteGameStateDialogue(state));
    }

    private void CompleteGameStateDialogue(GameStateData state) {
      relationshipManager.RankUpCatInGameState(dialogueData);
      if (!HasDialogueForCurrentState()) {
        //TODO(dwong): add non required game state dialogue
        objectiveManager.CompleteTask(task);
      }
      InRange();
    }
    
    private void StartRelationshipDialogue() {
      var rank = dialogueData.Rank;
      var dialogue = catDialogues.dialoguesForRank[rank].dialogue;
      dialogueManager.StartDialogue(dialogue, DialogueType.SPEECH, transform, this, CompleteRankDialogue);
    }
    
    private void CompleteRankDialogue() {
      relationshipManager.RankUpCat(dialogueData);
      InRange();
    }
    
    private CatDialogueForState GetDialogueForState(GameStateData state) {
      return catDialogues.dialoguesForStates.FirstOrDefault(dialogue => dialogue.gameStateData == state);
    }

    [YarnCommand("toScene")]
    public void ToScene(string dest) {
      sceneTransitionManager.TransitionToScene(dest);
    }

    public void UpdateElement(float animationProgress) { }

    public void OnActivate() { }

    public void OnDeactivate() { }
  }
  
  [Serializable]
  public class CatDialogueForState {
    public GameStateData gameStateData;
    public List<SerializedDialogue> catDialogue;
  }
  
  [Serializable]
  public class CatDialogues {
    public List<CatDialogueForState> dialoguesForStates;
    public List<SerializedDialogue> dialoguesForRank;
  }
}