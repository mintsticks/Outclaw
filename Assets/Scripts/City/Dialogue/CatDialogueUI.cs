﻿using System;
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
    
    private OptionChooser SetSelectedOption;
    private Transform bubbleParent;

    public Transform BubbleParent {
      set => bubbleParent = value;
    }

    public override IEnumerator RunLine(Line line) {
      var bubble = speechBubbleFactory.Create(new SpeechBubble.Data() {
        BubbleText = "", 
        BubbleParent = bubbleParent
      });
      bubble.transform.SetParent(transform);
      
      var text = ReplaceVariables(line.text);
      if (textSpeed > 0.0f) {
        var stringBuilder = new StringBuilder();
        foreach (var c in text) {
          stringBuilder.Append(c);
          bubble.SetText(stringBuilder.ToString());
          yield return new WaitForSeconds(textSpeed);
        }
      } else {
        bubble.SetText(text);
      }

      while (!playerInput.IsInteract()) {
        yield return null;
      }
      bubble.RemoveTail();
      StartCoroutine(FadeBubble(bubble));
      yield return new WaitForEndOfFrame();
    }

    public override IEnumerator RunOptions(Options optionsCollection, OptionChooser optionChooser) {
      SetSelectedOption = optionChooser;
      var bubble = thoughtBubbleFactory.Create(new ThoughtBubble.Data {
        Options = ParseOptions(optionsCollection),
        OnSelect = SetOption
      });
      bubble.transform.SetParent(transform);

      while (SetSelectedOption != null) {
        yield return null;
      }

      bubble.ToBubble();
      StartCoroutine(FadeBubble(bubble));
    }

    private IEnumerator FadeBubble(Bubble bubble) {
      for (var t = 0f; t <= bubbleFadeTime; t += Time.deltaTime) {  
        bubble.SetOpacity(1 - bubbleFade.Evaluate(t / bubbleFadeTime));
        bubble.BubbleTransform.Translate(new Vector2(0, 1));
        yield return null;
      }
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

    private void SetOption(int selectedOption) {

      // TODO: hotfix, probably do something else
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
      yield break;
    }
    
    public override IEnumerator DialogueComplete() {
      yield break;
    }
  }
}