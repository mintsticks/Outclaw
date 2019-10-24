using System;
using System.Collections.Generic;
using System.Linq;
using City;
using Outclaw.Heist;
using UnityEngine;
using Utility;
using Zenject;
using IObjectiveManager = City.IObjectiveManager;

namespace Outclaw.City {
  public class InteractableObject : MonoBehaviour, ObjectiveInteractable {
    [SerializeField] private Indicator observeIndicator;
    [SerializeField] private ObjectDialogues objectInfo;
    [SerializeField] private LocationData location;
    [SerializeField] private ObjectType objectType;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform objectPosition;
    [SerializeField] private ParticleSystem particleSystem;

    [Inject] private ILocationManager locationManager;
    [Inject] private IDialogueManager dialogueManager;
    [Inject] private IObjectiveManager objectiveManager;
    [Inject] private IObjectiveTransformManager objectiveTransformManager;
    [Inject] private IGameStateManager gameStateManager;
    [Inject] private IPlayer player;
    [Inject] private ISenseVisuals senseVisuals;

    private Transform parent;
    private bool created;

    public ObjectType ObjectType => objectType;

    public void Awake() {
      objectiveTransformManager.Objects.Add(this);
      senseVisuals.RegisterSenseElement(this);
    }

    public Transform ObjectPosition => objectPosition != null ? objectPosition : transform;

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
      dialogueManager.SetDialogueType(DialogueType.THOUGHT);
      dialogueManager.SetDialogue(dialogue);
      dialogueManager.SetBubbleParent(player.PlayerTransform);
      dialogueManager.StartDialogue(CompleteInteraction);
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

      var objProgress = locationManager.GetProgressForLocationObject(location, objectType);
      var objectDialogues = dialoguesForState.objectDialogues;
      if (objProgress >= objectDialogues.Count) {
        Debug.Log("No dialogues for object progress");
        return null;
      }

      return objectDialogues[objProgress].dialogue;
    }

    private ObjectDialogueForState GetDialogueForState(GameStateData state) {
      return objectInfo.dialoguesForStates.FirstOrDefault(dfs => dfs.gameStateData == state);
    }

    private void CompleteInteraction() {
      locationManager.IncreaseProgressForLocationObject(location, objectType);
      objectiveManager.CompleteObjectObjective(objectType);
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
  
  public enum ObjectType {
    NONE = 0,
    BED = 1,
    SHOWER = 2,
    DESK = 3,
    TRAPDOOR = 4,
    TABLE = 5,
    CABINET = 6,
    COLLAR = 7,
    NO_ANIMAL_SIGN = 8,
    GUM_WRAP = 9,
    AKIO_TOY = 10,
    RED_FABRIC = 11,
    ROPE = 12,
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