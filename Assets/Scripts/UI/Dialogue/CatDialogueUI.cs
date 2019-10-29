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
    [SerializeField]
    private float textSpeed = 0.025f;

    [SerializeField]
    private float bubbleFadeTime;

    [SerializeField]
    private AnimationCurve bubbleFade;

    [SerializeField]
    private List<DialogueVariable> dialogueVariables;
    
    [Inject]
    private SpeechBubble.Factory speechBubbleFactory;

    [Inject]
    private ThoughtBubble.Factory thoughtBubbleFactory;
    
    [Inject]
    private IPlayerInput playerInput;

    [Inject] 
    private IPauseGame pause;

    [Inject]
    private IPlayer player;
    
    private OptionChooser SetSelectedOption;
    private Transform bubbleParent;
    private Action onDialogueComplete;
    private DialogueType dialogueType;
    private HashSet<Bubble> bubbles = new HashSet<Bubble>();
    private bool skip = false;
    public Action OnDialogueComplete {
      set => onDialogueComplete = value;
    }
    
    public Transform BubbleParent {
      set => bubbleParent = value;
    }

    public DialogueType DialogueType {
      set => dialogueType = value;
    }
    
    public override IEnumerator RunLine(Line line) {
      var lineText = line.text;
      var parent = bubbleParent;
      if (HasMultipleText(lineText)) {
        lineText = ParseMultipleText(lineText);
        parent = player.PlayerTransform;
      }
      
      var bubble = speechBubbleFactory.Create(new SpeechBubble.Data() {
        BubbleText = "", 
        BubbleParent = parent,
        Type = dialogueType
      });
      bubble.transform.SetParent(transform, false);
      bubble.UpdatePosition();
      bubbles.Add(bubble);
      var text = ReplaceVariables(lineText);
      yield return ShowText(bubble, text);

      while (!IsValidDialogueProgression()) {
        yield return null;
      }
      bubble.RemoveTail();
      bubble.StartCoroutine(FadeBubble(bubble)); // make bubble own coroutine so it's never stopped
      yield return new WaitForEndOfFrame();
    }

    private IEnumerator ShowText(SpeechBubble bubble, string text) {
      if (textSpeed <= 0.0f) {
        bubble.SetText(text);
        yield return new WaitForEndOfFrame();
        yield break;
      } 
      
      var stringBuilder = new StringBuilder();
      skip = false;
      var detectSkip = DetectSkip();
      StartCoroutine(detectSkip);
      foreach (var c in text) {
        if (skip) {
          bubble.SetText(text);
          skip = false;
          yield return new WaitForEndOfFrame();
          yield break;
        }

        stringBuilder.Append(c);
        bubble.SetText(stringBuilder.ToString());
        yield return new WaitForSeconds(textSpeed);
      }  
      StopCoroutine(detectSkip);
    }

    private IEnumerator DetectSkip() {
      while (true) {
        if (!playerInput.IsInteractDown()) {
          yield return null;
          continue;
        } 

        skip = true;
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
        bubble.BubbleTransform.Translate(new Vector2(0, Time.deltaTime * 10.0f));
        yield return null;
      }

      bubbles.Remove(bubble);
      Destroy(bubble.BubbleTransform.gameObject);
    }

    private string ReplaceVariables (string input) {
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
      if(SetSelectedOption == null){
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