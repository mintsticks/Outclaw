using System;
using System.Collections.Generic;
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
    TABLE
  }

  [Serializable]
  public class ObjectInfo {
    [Tooltip("Object dialogues are different depending on game progression. " +
             "The dialogue is supplied in order that they are found in this list.")]
    public List<ObjectDialogue> dialoguesByProgress;
  }
  
  [Serializable]
  public class ObjectDialogue {
    [Tooltip("All the dialogues for an object, for a certain point in the game.")]
    public List<SerializedDialogue> objectDialogues;
  }
  
  public class InteractableObject : MonoBehaviour, Interactable {
    [SerializeField]
    private Indicator observeIndicator;

    [SerializeField]
    private ObjectInfo objectInfo;
    
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
    private IPlayer player;
    
    private Transform parent;
    private bool created;
    
    public void Awake() {
      observeIndicator.Initialize(transform);
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
      //TODO: add a key to the list elements for game states
      var locProgress = locationManager.GetProgressForLocation(locationType);
      var objProgress = locationManager.GetProgressForLocationObject(locationType, objectType);
      
      var dialoguesByProgress = objectInfo.dialoguesByProgress;
      if (locProgress >= objectInfo.dialoguesByProgress.Count) {
        Debug.Log("No dialogues for location progress");
        return null;
      }

      //TODO: add conditions to objects. for example, you cant trigger the 2nd dialogue for the trapdoor until youve picked up the key
      var objectDialogues = dialoguesByProgress[locProgress].objectDialogues;
      if (objProgress >= objectDialogues.Count) {
        Debug.Log("No dialogues for object progress");
        return null;
      }

      return objectDialogues[objProgress].dialogue;
    }
    
    private void CompleteInteraction() {
      locationManager.IncreaseProgressForLocationObject(locationType, objectType);
      //TODO: only complete objective if key dialogue is seen.
      objectiveManager.CompleteObjective(objectType);
      InRange();
    }
  }
}