using System;
using System.Collections;
using System.Collections.Generic;
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
    private PromptDisplay.Factory promptFactory;
    
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
      yield return BlockUntilStill();
      foreach (var eventInfo in events) {
        player.InputDisabled = true;
        yield return HandleEvent(eventInfo);
      }

      player.InputDisabled = false;
    }

    private IEnumerator BlockUntilStill() {
      player.InputDisabled = true;
      while (!player.Velocity.IsZero()) {
        yield return null;
      }
    }
    
    private IEnumerator HandleEvent(EventInfo eventInfo) {
      switch (eventInfo.eventType) {
        case EventType.WAIT:
          yield return new WaitForSeconds(eventInfo.waitTime);
          break;
        case EventType.PROMPT:
          yield return HandlePrompt(eventInfo.promptInfo);
          break;
        case EventType.DIALOGUE:
          yield return HandleDialogue(eventInfo.dialogue);
          break;
      }
    }
    
    private IEnumerator HandlePrompt(PromptData prompt) {
      var promptObj = promptFactory.Create(new PromptDisplay.Data { Info = prompt });
      while (!promptObj.IsDismissed) {
        yield return null;
      }
      
      Destroy(promptObj.gameObject);
    }

    private IEnumerator HandleDialogue(TextAsset[] dialogue) {
      dialogueManager.StartDialogue(dialogue, DialogueType.THOUGHT, player.HeadTransform);
      while (dialogueManager.IsDialogueRunning) {
        yield return null;
      }
    }
  }

  [Serializable]
  public class EventInfo { 
    public EventType eventType;
    public TextAsset[] dialogue;
    public int waitTime;
    public PromptData promptInfo;
  }
  
  public enum EventType {
    NONE,
    WAIT,
    PROMPT,
    DIALOGUE
  }
}