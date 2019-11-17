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
    [SerializeField] private List<DialogueVariable> dialogueVariables;
    [SerializeField] private Canvas canvas;

    [Inject] private SpeechBubble.Factory speechBubbleFactory;
    [Inject] private IconBubble.Factory iconBubbleFactory;
    [Inject] private ThoughtBubble.Factory thoughtBubbleFactory;
    [Inject] private IPlayerInput playerInput;
    [Inject] private IPauseGame pause;
    [Inject] private IPlayer player;

    private OptionChooser SetSelectedOption;
    private Transform bubbleParent;
    private Action onDialogueComplete;
    private Dictionary<Transform, Vector3?> bubblePositionsForParent = new Dictionary<Transform, Vector3?>();
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
    
    public ObjectiveInteractable CurrentInteractable {
      set => currentInteractable = value;
    }

    public override IEnumerator RunLine(Line line) {
      var lineText = line.text;
      var parent = bubbleParent;
      var bounds = new List<Bounds>();
      
      if (HasMultipleText(lineText)) {
        lineText = ParseMultipleText(lineText);
        parent = player.HeadTransform;
      }
      
      if (currentInteractable != null && currentInteractable.ObjectiveTransform != parent) {
        bounds.Add(currentInteractable.ObjectiveBounds);
      }
      if (parent != player.PlayerTransform && parent != player.HeadTransform) {
        bounds.Add(player.PlayerBounds);
      }
      
      if (HasIcon(lineText)) {
        yield return HandleIconBubble(parent, ParseIconName(lineText), bounds);
        yield break;
      }

      yield return HandleTextBubble(parent, lineText, bounds);
    }

    private IEnumerator HandleIconBubble(Transform parent, string key, List<Bounds> bounds) {
      var bubble = iconBubbleFactory.Create(new IconBubble.Data() {
        InvalidBounds = bounds,
        IconName = key,
        BubbleParent = parent,
        UIParent = transform,
        UI = this
      });
      bubbles.Add(bubble);
      
      while (!IsValidDialogueProgression()) {
        yield return null;
      }

      bubbles.Remove(bubble);
      bubble.StartCoroutine(bubble.FadeBubble()); // make bubble own coroutine so it's never stopped
      yield return new WaitForEndOfFrame();
    }
    
    private IEnumerator HandleTextBubble(Transform parent, string lineText, List<Bounds> bounds) {
      var bubble = CreateSpeechBubble(parent, bounds);
      if (!bubblePositionsForParent.ContainsKey(parent)) {
        bubblePositionsForParent[parent] = bubble.transform.position;
      }

      bubbles.Add(bubble);
      var text = ReplaceVariables(lineText);
      var detectSkip = DetectSkip(bubble);
      StartCoroutine(detectSkip);
      yield return bubble.ShowText(text);
      StopCoroutine(detectSkip);

      while (!IsValidDialogueProgression()) {
        yield return null;
      }

      bubbles.Remove(bubble);
      bubble.StartCoroutine(bubble.FadeBubble()); // make bubble own coroutine so it's never stopped
      yield return new WaitForEndOfFrame();
    }

    private SpeechBubble CreateSpeechBubble(Transform parent, List<Bounds> bounds) {
      if (!bubblePositionsForParent.ContainsKey(parent)) {
        return speechBubbleFactory.Create(new SpeechBubble.Data {
          BubbleText = "",
          BubbleParent = parent,
          UIParent = transform,
          InvalidBounds = bounds,
          UI = this
        });
      }
      return speechBubbleFactory.Create(new SpeechBubble.Data {
        InitialPosition = bubblePositionsForParent[parent],
        UIParent = transform,
        BubbleParent = parent,
        UI = this
      });
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
      var bounds = new List<Bounds>();
      if (currentInteractable != null) {
        bounds.Add(currentInteractable.ObjectiveBounds);
      }

      SetSelectedOption = optionChooser;
      var bubble = CreateThoughtBubble(optionsCollection, player.HeadTransform, bounds);
      bubbles.Add(bubble);

      while (SetSelectedOption != null) {
        yield return null;
      }

      bubbles.Remove(bubble);
      bubble.StartCoroutine(bubble.FadeBubble());
      yield return new WaitForEndOfFrame();
    }


    private ThoughtBubble CreateThoughtBubble(Options options, Transform parent, List<Bounds> bounds) {
      if (!bubblePositionsForParent.ContainsKey(parent)) {
        return thoughtBubbleFactory.Create(new ThoughtBubble.Data {
          Options = ParseOptions(options),
          OnSelect = SetOption,
          BubbleText = "",
          BubbleParent = player.HeadTransform,
          UIParent = transform,
          InvalidBounds = bounds,
          UI = this
        });
      }
      return thoughtBubbleFactory.Create(new ThoughtBubble.Data {
        Options = ParseOptions(options),
        OnSelect = SetOption,
        UIParent = transform,
        InitialPosition = bubblePositionsForParent[parent],
        BubbleParent = parent,
        UI = this
      });
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

    private bool HasIcon(string text) {
      return text.ToCharArray()[0] == '=';
    }

    private string ParseIconName(string text) {
      return text.Substring(1);
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

      bubblePositionsForParent.Clear();
      onDialogueComplete.Invoke();
      onDialogueComplete = null;
      
    }
  }
}