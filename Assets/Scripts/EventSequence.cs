using System;
using System.Collections;
using System.Collections.Generic;
using ModestTree;
using Outclaw.City;
using UnityEngine;
using Zenject;

namespace Outclaw {
  public class EventSequence : MonoBehaviour {
    [SerializeField]
    private List<EventInfo> events;

    [SerializeField]
    private GameStateData eventGameState;
    
    [Inject]
    private DismissablePromptFactory dismissablePromptFactory;
    
    [Inject]
    private IDialogueManager dialogueManager;

    [Inject]
    private IPlayer player;

    [Inject]
    private IGameStateManager gameStateManager;

    private bool executed;
    
    public IEnumerator ExecuteSequence() {
      if (executed || gameStateManager.CurrentGameStateData != eventGameState) {
        yield break;
      }

      executed = true;
      foreach (var eventInfo in events) {
        yield return HandleEvent(eventInfo);
      }
    }

    private IEnumerator HandleEvent(EventInfo eventInfo) {
      switch (eventInfo.eventType) {
        case EventType.WAIT:
          yield return new WaitForSeconds(eventInfo.waitTime);
          break;
        case EventType.PROMPT:
          yield return HandlePrompt(eventInfo.promptType, eventInfo.promptText);
          break;
        case EventType.DIALOGUE:
          yield return HandleDialogue(eventInfo.dialogue);
          break;
        case EventType.DIALOGUE_PROMPT:
          StartCoroutine(HandlePrompt(eventInfo.promptType, eventInfo.promptText));
          yield return HandleDialogue(eventInfo.dialogue);
          break;
        case EventType.INTERACT_ON:
          break;
        case EventType.INTERACT_OFF:
          break;
      }
    }
    
    private IEnumerator HandlePrompt(PromptType prompt, string promptText) {
      var promptObj = dismissablePromptFactory.Create(prompt);
      Debug.Log("i made a prompt");
      if (!promptText.IsEmpty()) {
        promptObj.SetPromptText(promptText);
      }
      
      while (!promptObj.IsDismissable()) {
        yield return null;
      }
      StartCoroutine(promptObj.DismissPrompt());
    }

    private IEnumerator HandleDialogue(TextAsset[] dialogue) {
      //TODO(dwong): handle dialgue event involving other cats
      dialogueManager.SetDialogueType(DialogueType.THOUGHT);
      dialogueManager.SetDialogue(dialogue);
      dialogueManager.SetBubbleParent(player.PlayerTransform);
      dialogueManager.StartDialogue();
      while (dialogueManager.IsDialogueRunning) {
        yield return null;
      }
    }
  }

  [Serializable]
  public class EventInfo {
    //Unity doesn't support serialized interfaces, so we have to combine all fields into this class.
    public EventType eventType;
    public TextAsset[] dialogue;
    public int waitTime;
    public PromptType promptType;
    public string promptText;
  }
  
  public enum EventType {
    NONE,
    WAIT,
    PROMPT,
    DIALOGUE,
    DIALOGUE_PROMPT,
    INTERACT_OFF,
    INTERACT_ON,
  }
}