using System;
using System.Collections.Generic;
using System.Linq;
using City;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public enum EntranceType {
    NONE = 0,
    CAFE_EXIT = 1,
    CAFE_ENTRANCE = 2,
    HOME_EXIT = 3,
    HOME_ENTRANCE = 4,
    PARK_ENTRANCE = 5,
    POUND_ENTRANCE = 6,
    PARK_EXIT = 7
  }

  [Serializable]
  public class LocationDialogueForState {
    public GameStateType gameState;
    public bool isBlocking;
    public SerializedDialogue locationDialogue;
  }

  public class InteractableLocation : MonoBehaviour, ObjectiveInteractable {
    [SerializeField] private Indicator enterIndicator;
    [SerializeField] private List<LocationDialogueForState> locationDialoguesForState;
    [SerializeField] private LocationData destinationLocation;
    [SerializeField] private EntranceType entranceType;
    [SerializeField] private AudioClip enterClip;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform locationPosition;
    [SerializeField] private ParticleSystem particleSystem;

    [Inject] private IPlayer player;
    [Inject] private IDialogueManager dialogueManager;
    [Inject] private ISoundManager soundManager;
    [Inject] private ISceneTransitionManager sceneTransitionManager;
    [Inject] private IGameStateManager gameStateManager;
    [Inject] private IObjectiveManager objectiveManager;
    [Inject] private IObjectiveTransformManager objectiveTransformManager;
    [Inject] private ISenseVisuals senseVisuals;

    public EntranceType Type => entranceType;
    public Transform LocationPosition => locationPosition != null ? locationPosition : transform;

    public void Awake() {
      objectiveTransformManager.Locations.Add(this);
      senseVisuals.RegisterSenseElement(this);
    }

    public void InRange() {
      enterIndicator.FadeIn();
    }

    public void ExitRange() {
      enterIndicator.FadeOut();
    }

    public void Interact() {
      var locationDialogueForState = GetDialogueForState(gameStateManager.CurrentGameState);
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
      dialogueManager.SetDialogueType(DialogueType.THOUGHT);
      dialogueManager.SetDialogue(locationDialogueForState.locationDialogue.dialogue);
      dialogueManager.SetBubbleParent(player.PlayerTransform);
      dialogueManager.StartDialogue(() => CompleteDialogue(!locationDialogueForState.isBlocking));
    }

    private void CompleteDialogue(bool enter) {
      if (!enter) {
        InRange();
        return;
      }

      EnterLocation();
    }

    private void EnterLocation() {
      if (enterClip != null) {
        soundManager.PlaySFX(enterClip);
      }

      objectiveManager.CompleteEntranceObjective(entranceType);
      sceneTransitionManager.TransitionToScene(destinationLocation);
    }

    private LocationDialogueForState GetDialogueForState(GameStateType state) {
      return locationDialoguesForState.FirstOrDefault(dialogue => dialogue.gameState == state);
    }

    public void UpdateElement(float animationProgress) { }
    public void OnActivate() { }

    public void OnDeactivate() { }
  }
}