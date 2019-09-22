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
  public class CatDialogueUI : DialogueUIBehaviour {
    [SerializeField]
    private float textSpeed = 0.025f;
    
    [SerializeField]
    private CatVariableStorage variableStorage;

    [SerializeField]
    private float bubbleFadeTime;

    [SerializeField]
    private AnimationCurve bubbleFade;
    
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

    public override IEnumerator RunLine(Yarn.Line line) {
      //Create the bubble?
      var bubble = speechBubbleFactory.Create(new SpeechBubble.Data() {
        BubbleText = "", 
        BubbleParent = bubbleParent
      });
      bubble.transform.SetParent(transform);
      
      var text = CheckVars(line.text);
      
      if (textSpeed > 0.0f) {
        // Display the line one character at a time
        var stringBuilder = new StringBuilder();

        foreach (char c in text) {
          stringBuilder.Append(c);
          bubble.SetText(stringBuilder.ToString());
          yield return new WaitForSeconds(textSpeed);
        }
      } else {
        // Display the line immediately if textSpeed == 0
        bubble.SetText(text);
      }

      while (!playerInput.IsInteract()) {
        yield return null;
      }
      
      StartCoroutine(FadeBubble(bubble));
      yield return new WaitForEndOfFrame();
    }
    
    private IEnumerator FadeBubble(SpeechBubble bubble) {
      bubble.RemoveTail();
      for (var t = 0f; t <= bubbleFadeTime; t += Time.deltaTime) {  
        bubble.SetOpacity(1 - bubbleFade.Evaluate(t / bubbleFadeTime));
        bubble.transform.Translate(new Vector2(0, 1));
        yield return null;
      }
      Destroy(bubble.gameObject);
    }
    
    private string CheckVars(string input) {
      var output = string.Empty;
      var checkingVar = false;
      var currentVar = string.Empty;

      var index = 0;
      while (index < input.Length) {
        if (input[index] == '[') {
          checkingVar = true;
          currentVar = string.Empty;
        } else if (input [index] == ']') {
          checkingVar = false;
          output += ParseVariable(currentVar);
          currentVar = string.Empty;
        } else if (checkingVar) {
          currentVar += input [index];
        } else {
          output += input[index];
        }
        index++;
      }

      return output;
    }

    string ParseVariable (string varName) {
      //Check YarnSpinner's variable storage first
      if (variableStorage.GetValue (varName) != Value.NULL) {
        return variableStorage.GetValue (varName).AsString;
      }

      //Handle other variables here
      if(varName == "$name") {
        //TODO(dwong): replace with actual name.
        return "Kupo";
      }

      //If no variables are found, return the variable name
      return varName;
    }

    public override IEnumerator RunOptions(Options optionsCollection, OptionChooser optionChooser) {
      SetSelectedOption = optionChooser;
      var thoughtBubble = thoughtBubbleFactory.Create(new ThoughtBubble.Data {
        Options = ParseOptions(optionsCollection),
        OnSelect = SetOption
      });
      thoughtBubble.transform.SetParent(transform);

      while (SetSelectedOption != null) {
        yield return null;
      }

      StartCoroutine(FadeThoughtBubble(thoughtBubble));
    }

    private List<string> ParseOptions(Options optionsCollection) {
      return optionsCollection.options.Select(CheckVars).ToList();
    }
    
    private IEnumerator FadeThoughtBubble(ThoughtBubble bubble) {
      bubble.ToBubble();
      for (var t = 0f; t <= bubbleFadeTime; t += Time.deltaTime) {  
        bubble.SetOpacity(1 - bubbleFade.Evaluate(t / bubbleFadeTime));
        bubble.transform.Translate(new Vector2(0, 1));
        yield return null;
      }
      Destroy(bubble.gameObject);
    }

    /// Called by buttons to make a selection.
    public void SetOption(int selectedOption) {
      // Call the delegate to tell the dialogue system that we've
      // selected an option.
      SetSelectedOption(selectedOption);

      // Now remove the delegate so that the loop in RunOptions will exit
      SetSelectedOption = null;
    }

    /// Run an internal command.
    public override IEnumerator RunCommand(Yarn.Command command) {
      // "Perform" the command
      Debug.Log("Command: " + command.text);

      yield break;
    }

    /// Called when the dialogue system has started running.
    public override IEnumerator DialogueStarted() {
      yield break;
    }

    /// Called when the dialogue system has finished running.
    public override IEnumerator DialogueComplete() {
      //TODO(dwong): Script should start a minigame, Start the minigame, and second part of dialogue? Or 
      yield break;
    }
  }
}