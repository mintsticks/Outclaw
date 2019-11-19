﻿using System;
using System.Collections.Generic;
using System.Linq;
using City;
using Outclaw.Heist;
using UnityEngine;
using Utility;
using Zenject;
using IObjectiveManager = City.IObjectiveManager;

namespace Outclaw.City {
  public class InteractableObject : MonoBehaviour, ObjectiveInteractable, IHaveTask {
    [SerializeField] private Indicator observeIndicator;
    [SerializeField] private ObjectDialogues objectInfo;
    [SerializeField] private ObjectDialogueData dialogueData;
    [SerializeField] private BoxCollider2D bound;
    [SerializeField] private Task promptTask;
    
    [Header("Effects")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private ParticleSystem particleSystem;

    [Header("Objective Tracking")]
    [SerializeField] private Task task;

    [Inject] private ILocationManager locationManager;
    [Inject] private IDialogueManager dialogueManager;
    [Inject] private IObjectiveManager objectiveManager;
    [Inject] private IObjectiveTransformManager objectiveTransformManager;
    [Inject] private IGameStateManager gameStateManager;
    [Inject] private IPlayer player;
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

      observeIndicator.FadeIn();
    }

    public void ExitRange() {
      observeIndicator.FadeOut();
    }

    public void Interact() {
      if (!HasInteraction()) {
        return;
      }

      var dialogue = GetObjectDialogue();
      observeIndicator.FadeOut();
      dialogueManager.StartDialogue(dialogue, DialogueType.SPEECH, player.HeadTransform, this, CompleteInteraction);
      if (promptTask != null && !promptTask.IsComplete) {
        promptTask.Complete();
      }
    }

    public bool HasInteraction() {
      return GetObjectDialogue() != null;
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

    private TextAsset[] GetObjectDialogue() {
      var gameState = gameStateManager.CurrentGameStateData;
      var dialoguesForState = GetDialogueForState(gameState);
      if (dialoguesForState == null) {
        Debug.Log("No dialogues for object in current game state: " + gameState);
        return null;
      }

      var objProgress = dialogueData.Progress;
      var objectDialogues = dialoguesForState.objectDialogues;
      if (objProgress >= objectDialogues.Count) {
        Debug.Log("No dialogues for object progress" + objProgress);
        return null;
      }

      return objectDialogues[objProgress].dialogue;
    }

    private ObjectDialogueForState GetDialogueForState(GameStateData state) {
      return objectInfo.dialoguesForStates.FirstOrDefault(dfs => dfs.gameStateData == state);
    }

    private void CompleteInteraction() {
      locationManager.IncreaseProgress(dialogueData);
      objectiveManager.CompleteTask(task);
      InRange();
    }

    public void UpdateElement(float animationProgress) {
      if (HasInteraction()) {
        return;
      }
      spriteRenderer.material.SetFloat(GlobalConstants.GREY_EFFECT_NAME, animationProgress);
    }

    public void OnActivate() {
      if (!HasInteraction()) {
        return;
      }
      EnableEffect();
    }

    public void OnDeactivate() {
      DisableEffect();
    }
  }
  
  [Serializable]
  public class ObjectDialogues {
    [Tooltip("Object dialogues are different depending on game state.")]
    public List<ObjectDialogueForState> dialoguesForStates;
  }

  [Serializable]
  public class ObjectDialogueForState {
    public GameStateData gameStateData;

    [Tooltip("All the dialogues for an object, for a certain gamestate.")]
    public List<SerializedDialogue> objectDialogues;
  }
}