using System;
using System.Collections.Generic;
using System.Linq;
using City;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public enum EntranceType {
    CAFE_EXIT,
    CAFE_ENTRANCE,
    HOME_EXIT,
  }
  
  [Serializable]
  public class LocationDialogueForState {
    public GameStateType gameState;
    public bool isBlocking;
    public SerializedDialogue locationDialogue;
  }

  public class InteractableLocation : MonoBehaviour, CityInteractable {
    [SerializeField]
    private Indicator enterIndicator;

    [SerializeField]
    private List<LocationDialogueForState> locationDialoguesForState;
    
    [SerializeField]
    private string destinationName;

    [SerializeField]
    private EntranceType entranceType;
    
    [SerializeField]
    private AudioClip enterClip;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private ParticleSystem particleSystem;
    
    [Inject]
    private IPlayer player;

    [Inject]
    private IDialogueManager dialogueManager;
    
    [Inject]
    private ISoundManager soundManager;

    [Inject]
    private ISceneTransitionManager sceneTransitionManager;
    
    [Inject]
    private IGameStateManager gameStateManager;

    [Inject]
    private IObjectiveManager objectiveManager;
    
    [Inject]
    private IObjectiveTransformManager objectiveTransformManager;

    [Inject]
    private ISenseManager senseManager;
    
    public EntranceType Type => entranceType;

    public void Awake() {
      enterIndicator.Initialize(player.PlayerTransform);
      objectiveTransformManager.Locations.Add(this);
      senseManager.RegisterCityInteractable(this);
    }
    
    public void InRange() {
      enterIndicator.CreateIndicator();
      StartCoroutine(enterIndicator.FadeIn());
    }

    public void ExitRange() {
      StartCoroutine(enterIndicator.FadeOut());
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

    public SpriteRenderer GetSpriteRenderer() {
      return spriteRenderer;
    }
    
    private void HandleDialogue(LocationDialogueForState locationDialogueForState) {
      StartCoroutine(enterIndicator.FadeOut());
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
      if(enterClip != null){
        soundManager.PlaySFX(enterClip);
      }
      
      objectiveManager.CompleteEntranceObjective(entranceType);
      sceneTransitionManager.TransitionToScene(destinationName);
    }

    private LocationDialogueForState GetDialogueForState(GameStateType state) {
      return locationDialoguesForState.FirstOrDefault(dialogue => dialogue.gameState == state);
    }
  }
}