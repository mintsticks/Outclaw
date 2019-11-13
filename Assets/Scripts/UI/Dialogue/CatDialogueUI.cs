using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Outclaw.City;
using UnityEngine;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;
using Zenject;

namespace Outclaw {
  [Serializable]
  public class DialogueVariable {
    public string key;
    public string value;
  }

  public class CatDialogueUI : DialogueUIBehaviour {
    [SerializeField] private float bubbleFadeTime;
    [SerializeField] private AnimationCurve bubbleFade;
    [SerializeField] private List<DialogueVariable> dialogueVariables;
    [SerializeField] private Canvas canvas;
    
    [Inject] private SpeechBubble.Factory speechBubbleFactory;
    [Inject] private ThoughtBubble.Factory thoughtBubbleFactory;
    [Inject] private IPlayerInput playerInput;
    [Inject] private IPauseGame pause;
    [Inject] private IPlayer player;

    private OptionChooser SetSelectedOption;
    private Transform bubbleParent;
    private Action onDialogueComplete;
    private DialogueType dialogueType;
    private HashSet<Bubble> bubbles = new HashSet<Bubble>();
    private bool skip;
    private ObjectiveInteractable currentInteractable;

    public Canvas DialogueCanvas => canvas;

    public Action OnDialogueComplete {
      set => onDialogueComplete = value;
    }

    public Transform BubbleParent {
      set => bubbleParent = value;
    }

    public DialogueType DialogueType {
      set => dialogueType = value;
    }

    public ObjectiveInteractable CurrentInteractable {
      set => currentInteractable = value;
    }

    public override IEnumerator RunLine(Line line) {
      var lineText = line.text;
      var parent = bubbleParent;
      if (HasMultipleText(lineText)) {
        lineText = ParseMultipleText(lineText);
        parent = player.PlayerTransform;
      }

      var bounds = new List<Bounds>();
      if (currentInteractable != null) {
        bounds.Add(currentInteractable.ObjectiveBounds);
      }
      
      var bubble = speechBubbleFactory.Create(new SpeechBubble.Data() {
        BubbleText = "",
        BubbleParent = parent,
        UIParent = transform,
        Type = dialogueType,
        InvalidBounds = bounds,
        UI = this
      });
      
      bubbles.Add(bubble);
      var text = ReplaceVariables(lineText);
      var detectSkip = DetectSkip(bubble);
      StartCoroutine(detectSkip);
      yield return bubble.ShowText(text);
      StopCoroutine(detectSkip);
      
      while (!IsValidDialogueProgression()) {
        yield return null;
      }
      
      bubble.StartCoroutine(FadeBubble(bubble)); // make bubble own coroutine so it's never stopped
      yield return new WaitForEndOfFrame();
    }
    
    private IEnumerator DetectSkip(SpeechBubble bubble) {
      while (true) {
        if (!playerInput.IsInteractDown()) {
          yield return null;
          continue;
        }

        bubble.SkipText();
        yield break;
      }
    }

    private bool IsValidDialogueProgression() {
      return playerInput.IsInteractDown() && !pause.IsPaused;
    }

    public override IEnumerator RunOptions(Options optionsCollection, OptionChooser optionChooser) {
      SetSelectedOption = optionChooser;
      var bubble = thoughtBubbleFactory.Create(new ThoughtBubble.Data {
        Options = ParseOptions(optionsCollection),
        OnSelect = SetOption
      });
      bubble.transform.SetParent(transform, false);
      bubble.UpdatePosition();
      bubbles.Add(bubble);

      while (SetSelectedOption != null) {
        yield return null;
      }

      bubble.ToBubble();
      StartCoroutine(FadeBubble(bubble));
      yield return new WaitForEndOfFrame();
    }

    private IEnumerator FadeBubble(Bubble bubble) {
      for (var t = 0f; t <= bubbleFadeTime; t += Time.deltaTime) {
        if (bubble == null) {
          yield break;
        }

        bubble.SetOpacity(1 - bubbleFade.Evaluate(t / bubbleFadeTime));
        yield return null;
      }

      bubbles.Remove(bubble);
      Destroy(bubble.BubbleTransform.gameObject);
    }

    private string ReplaceVariables(string input) {
      foreach (var variable in dialogueVariables) {
        input = input.Replace(variable.key, variable.value);
      }

      return input;
    }

    private List<string> ParseOptions(Options optionsCollection) {
      return optionsCollection.options.Select(ReplaceVariables).ToList();
    }

    private bool HasMultipleText(string text) {
      return text.ToCharArray()[0] == '>';
    }

    private string ParseMultipleText(string text) {
      return text.Substring(1);
    }

    private void SetOption(int selectedOption) {
      if (SetSelectedOption == null) {
        return;
      }

      SetSelectedOption(selectedOption);
      SetSelectedOption = null;
    }

    public override IEnumerator RunCommand(Command command) {
      yield break;
    }

    public override IEnumerator DialogueStarted() {
      foreach (var bubble in bubbles) {
        Destroy(bubble.BubbleTransform.gameObject);
      }

      bubbles.Clear();
      yield break;
    }

    public override IEnumerator DialogueComplete() {
      if (onDialogueComplete == null) {
        yield break;
      }

      onDialogueComplete.Invoke();
      onDialogueComplete = null;
    }
  }
}