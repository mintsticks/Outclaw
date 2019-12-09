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
    public SerializedDialogue locationDialogue;
  }

  public class InteractableLocation : MonoBehaviour, ObjectiveInteractable, IHaveTask {
    [SerializeField] private Indicator enterIndicator;
    [SerializeField] private LocationData destinationLocation;
    [SerializeField] private BoxCollider2D bound;
    [SerializeField] private Task promptTask;

    [Header("Dialogue")]
    [Tooltip("Overrides the bottom two. Always grants access for the listed states.")]
    [SerializeField] private List<LocationDialogueForState> entryDialogue;
    [Tooltip("Overrides total access. Denies access for the listed states.")]
    [SerializeField] private List<LocationDialogueForState> uniqueStateBlockingDialogue;
    [Tooltip("Putting dialogue here will make default action to deny entry.")]
    [SerializeField] private List<SerializedDialogue> defaultBlockingDialogue;
    private int defaultDialogueIdx;
    
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

    private bool runningDialogue;

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
      if(runningDialogue){
        return;
      }

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
      bool blocking = GetDialogueForState(gameStateManager.CurrentGameStateData,
        out SerializedDialogue locationDialogueForState);
      if (promptTask != null && !promptTask.IsComplete) {
        promptTask.Complete();
      }
      
      if (locationDialogueForState != null) {
        HandleDialogue(locationDialogueForState, !blocking);
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

    private void HandleDialogue(SerializedDialogue dialogue, bool enter) {
      enterIndicator.FadeOut();
      runningDialogue = true;

      dialogueManager.StartDialogue(dialogue.dialogue, 
        DialogueType.SPEECH,
        player.PlayerTransform, 
        this, 
        () => CompleteDialogue(enter));
    }

    private void CompleteDialogue(bool enter) {
      runningDialogue = false;
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

    // returns true if the dialogue should block entry
    private bool GetDialogueForState(GameStateData state, out SerializedDialogue res) {
      Func<LocationDialogueForState, bool> filter = 
        (LocationDialogueForState dialogue) => dialogue.gameStateData == state;
      res = entryDialogue.FirstOrDefault(filter)?.locationDialogue;
      if(res != null){
        return false;
      }

      res = uniqueStateBlockingDialogue.FirstOrDefault(filter)?.locationDialogue;
      if(res != null){
        return true;
      }

      if(defaultBlockingDialogue.Count > 0){
        int index = ((defaultDialogueIdx + 1) < defaultBlockingDialogue.Count)
          ? defaultDialogueIdx++ : (defaultBlockingDialogue.Count - 1);
        res = defaultBlockingDialogue[index];
        return true;
      }

      return false;
    }

    public void UpdateElement(float animationProgress) { }
    public void OnActivate() { }

    public void OnDeactivate() { }
  }
}