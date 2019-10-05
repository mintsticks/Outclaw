using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public class PromptManager : MonoBehaviour{
    [SerializeField]
    private List<GameStatePrompts> gameStatePrompts;

    [Inject]
    private IPlayerInput playerInput;
    
    [Inject]
    private IGameStateManager gameStateManager;

    [Inject]
    private DismissablePromptFactory dismissablePromptFactory;

    private HashSet<GameStateType> completedPrompts;

    private void Awake() {
      completedPrompts = new HashSet<GameStateType>();
    }

    private void Update() {
      if (completedPrompts.Contains(gameStateManager.CurrentGameState)) {
        return;
      }
      StartCoroutine(ShowPromptsForCurrentState());
    }

    private IEnumerator ShowPromptsForCurrentState() {
      completedPrompts.Add(gameStateManager.CurrentGameState);
      var prompts = GetPromptsForState(gameStateManager.CurrentGameState);
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
    
    private List<PromptType> GetPromptsForState(GameStateType state) {
      var statePrompts = gameStatePrompts.FirstOrDefault(gsp => gsp.gameState == state);
      return statePrompts?.prompts;
    }
  }

  [Serializable]
  public class GameStatePrompts {
    public GameStateType gameState;
    public List<PromptType> prompts;
  }
}