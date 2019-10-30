using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public class PromptManager : MonoBehaviour {
    [SerializeField] private List<GameStatePrompts> gameStatePrompts;

    [SerializeField] private List<PromptInfo> prompts;
    
    [Inject]
    private IPlayerInput playerInput;
    
    [Inject]
    private IGameStateManager gameStateManager;

    [Inject]
    private DismissablePromptFactory dismissablePromptFactory;

    private HashSet<GameStateData> completedPrompts;

    private void Awake() {
      completedPrompts = new HashSet<GameStateData>();
    }

    private void Update() {
      if (completedPrompts.Contains(gameStateManager.CurrentGameStateData)) {
        return;
      }
      StartCoroutine(ShowPromptsForCurrentState());
    }

    private IEnumerator ShowPromptsForCurrentState() {
      completedPrompts.Add(gameStateManager.CurrentGameStateData);
      var prompts = GetPromptsForState(gameStateManager.CurrentGameStateData);
      foreach (var prompt in prompts) {
        yield return new WaitForSeconds(5);
        yield return HandlePrompt(prompt);
      }
    }

    private IEnumerator HandlePrompt(PromptType prompt) {
      var promptObj = dismissablePromptFactory.Create(prompt);
      while (!promptObj.IsDismissable()) {
        yield return null;
      }
      promptObj.DismissPrompt();
    }
    
    private List<PromptType> GetPromptsForState(GameStateData state) {
      var statePrompts = gameStatePrompts.FirstOrDefault(gsp => gsp.gameState == state);
      return statePrompts?.prompts;
    }
  }

  [Serializable]
  public class GameStatePrompts {
    public GameStateData gameState;
    public List<PromptType> prompts;
  }

  [Serializable]
  public class PromptInfo {
    public GameStateData promptGameState;
    public PromptInfoType promptInfoType;
    public PromptInfoType nextPromptType;
    public Sprite promptImage;
    public string promptTitle;
    public string promptDescription;
    public bool hasImage;
    public bool hasTitle;
  }

  public enum PromptInfoType {
    NONE = 0,
    MOVE = 1,
    JUMP = 2,
    INTERACT = 3,
    SOUND_DANGER = 4,
    SNEAK = 5,
    SENSE = 6,
  }
}