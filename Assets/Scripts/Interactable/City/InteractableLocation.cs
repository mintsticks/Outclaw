using System;
using System.Collections.Generic;
using System.Linq;
using City;
using UnityEngine;
using Zenject;

namespace Outclaw.City {

  [Serializable]
  public class LocationDialogueForState {
    public GameStateData gameStateData;
    public bool isBlocking;
    public SerializedDialogue locationDialogue;
  }

  public class InteractableLocation : MonoBehaviour, ObjectiveInteractable, IHaveTask {
    [SerializeField] private Indicator enterIndicator;
    [SerializeField] private List<LocationDialogueForState> locationDialoguesForState;
    [SerializeField] private LocationData destinationLocation;
    [SerializeField] private BoxCollider2D bound;
    [SerializeField] private Task promptTask;
    
    [Header("Effects")]
    [SerializeField] private AudioClip enterClip;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform locationPosition;
    [SerializeField] private ParticleSystem particleSystem;

    [Header("Objective Tracking")]
    [SerializeField] private Task task;

    [Inject] private IPlayer player;
    [Inject] private IDialogueManager dialogueManager;
    [Inject] private ISoundManager soundManager;
    [Inject] private ISceneTransitionManager sceneTransitionManager;
    [Inject] private IGameStateManager gameStateManager;
    [Inject] private IObjectiveManager objectiveManager;
    [Inject] private IObjectiveTransformManager objectiveTransformManager;
    [Inject] private ISenseVisuals senseVisuals;

    public Transform LocationPosition => locationPosition != null ? locationPosition : transform;
    public Task ContainedTask => task;
    public Transform Location => transform;
    public LocationData Destination => destinationLocation;
    public Transform ObjectiveTransform => transform;

    public Bounds ObjectiveBounds {
      get {
        if (bound == null) {
          var bounds = new Bounds();
          bounds.center = transform.position;
          return bounds;
        }

        return bound.bounds;
      }
    }
    

    public void Awake() {
      objectiveTransformManager.RegisterTask(this);
      senseVisuals.RegisterSenseElement(this);
    }

    public void InRange(InteractableState state) {
      switch(state){
        case InteractableState.DisabledVisible:
          enterIndicator.FadeToDisabled();
          break;
        case InteractableState.Enabled:
          enterIndicator.FadeIn();
          break;
      }
    }

    public void ExitRange() {
      enterIndicator.FadeOut();
    }

    public void Interact() {
      var locationDialogueForState = GetDialogueForState(gameStateManager.CurrentGameStateData);
      if (promptTask != null && !promptTask.IsComplete) {
        promptTask.Complete();
      }
      
      if (locationDialogueForState != null) {
        HandleDialogue(locationDialogueForState);
        return;
      }

      EnterLocation();
    }

    public bool HasInteraction() {
      return true;
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

    private void HandleDialogue(LocationDialogueForState locationDialogueForState) {
      enterIndicator.FadeOut();
      dialogueManager.StartDialogue(locationDialogueForState.locationDialogue.dialogue, 
        DialogueType.SPEECH,
        player.PlayerTransform, 
        this, 
        () => CompleteDialogue(!locationDialogueForState.isBlocking));
    }

    private void CompleteDialogue(bool enter) {
      if (!enter) {
        // since player could interact, assume can still interact
        InRange(InteractableState.Enabled);
        return;
      }

      EnterLocation();
    }

    private void EnterLocation() {
      if (enterClip != null) {
        soundManager.PlaySFX(enterClip);
      }
      objectiveManager.CompleteTask(task);
      sceneTransitionManager.TransitionToScene(destinationLocation);
    }

    private LocationDialogueForState GetDialogueForState(GameStateData state) {
      return locationDialoguesForState.FirstOrDefault(dialogue => dialogue.gameStateData == state);
    }

    public void UpdateElement(float animationProgress) { }
    public void OnActivate() { }

    public void OnDeactivate() { }
  }
}