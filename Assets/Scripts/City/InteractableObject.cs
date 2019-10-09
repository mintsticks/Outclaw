using System;
using System.Collections.Generic;
using System.Linq;
using City;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public enum ObjectType {
    NONE,
    BED,
    SHOWER,
    DESK,
    TRAPDOOR,
    TABLE,
    CABINET
  }

  [Serializable]
  public class ObjectDialogues {
    [Tooltip("Object dialogues are different depending on game state.")]
    public List<ObjectDialogueForState> dialoguesForStates;
  }
  
  [Serializable]
  public class ObjectDialogueForState {
    public GameStateType gameState;
    [Tooltip("All the dialogues for an object, for a certain gamestate.")]
    public List<SerializedDialogue> objectDialogues;
  }
  
  public class InteractableObject : MonoBehaviour, Interactable {
    [SerializeField]
    private Indicator observeIndicator;

    [SerializeField]
    private ObjectDialogues objectInfo;
    
    [SerializeField]
    private LocationType locationType;
    
    [SerializeField]
    private ObjectType objectType;
    
    [Inject]
    private ILocationManager locationManager;

    [Inject]
    private IDialogueManager dialogueManager;

    [Inject]
    private IObjectiveManager objectiveManager;

    [Inject]
    private IObjectiveTransformManager objectiveTransformManager;

    [Inject]
    private IGameStateManager gameStateManager;
    
    [Inject]
    private IPlayer player;
    
    private Transform parent;
    private bool created;

    public ObjectType ObjectType => objectType;

    public void Awake() {
      observeIndicator.Initialize(transform);
      objectiveTransformManager.Objects.Add(this);
    }

    public void InRange() {
      if (GetObjectDialogue() == null) {
        return;
      }
      observeIndicator.CreateIndicator();
      StartCoroutine(observeIndicator.FadeIn());
    }

    public void ExitRange() {
      StartCoroutine(observeIndicator.FadeOut());
    }

    public void Interact() {
      var dialogue = GetObjectDialogue();
      if (dialogue == null) {
        return;
      }
      
      StartCoroutine(observeIndicator.FadeOut());
      dialogueManager.SetDialogueType(DialogueType.THOUGHT);
      dialogueManager.SetDialogue(dialogue);
      dialogueManager.SetBubbleParent(player.PlayerTransform);
      dialogueManager.StartDialogue(CompleteInteraction);
    }

    private TextAsset[] GetObjectDialogue() {
      var gameState = gameStateManager.CurrentGameState;
      var dialoguesForState = GetDialogueForState(gameState);
      if (dialoguesForState == null) {
        Debug.Log("No dialogues for object in current game state: " + gameState);
        return null;
      }

      var objProgress = locationManager.GetProgressForLocationObject(locationType, objectType);
      var objectDialogues = dialoguesForState.objectDialogues;
      if (objProgress >= objectDialogues.Count) {
        Debug.Log("No dialogues for object progress");
        return null;
      }

      return objectDialogues[objProgress].dialogue;
    }

    private ObjectDialogueForState GetDialogueForState(GameStateType state) {
      return objectInfo.dialoguesForStates.FirstOrDefault(dfs => dfs.gameState == state);
    }
    
    private void CompleteInteraction() {
      locationManager.IncreaseProgressForLocationObject(locationType, objectType);
      objectiveManager.CompleteObjectObjective(objectType);
      InRange();
    }
  }
}