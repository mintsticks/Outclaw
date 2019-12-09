using System;
using System.Collections;
using System.Text;
using Outclaw.City;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Zenject;

namespace Outclaw {
  public class BlackScreenDialogueUI : MonoBehaviour {
    [SerializeField] private Text introText;
    [SerializeField,  TextArea(3,20)] private string[] lines;
    [SerializeField] private BubbleTextHelper bubbleTextHelper;
    [SerializeField] private Canvas canvas;
    [SerializeField] private int fontSize;
    
    [SerializeField] [Tooltip("Called when text ends")]
    private UnityEvent onComplete;

    [Inject] private IPlayerInput playerInput;

    public void Start() {
      bubbleTextHelper.Initialize(canvas, null, fontSize);
      StartCoroutine(PrintLines(lines));
    }

    private IEnumerator PrintLines(string[] lines) {
      foreach (var line in lines) {
        yield return HandleLine(line);
      }

      onComplete.Invoke();
    }

    private IEnumerator HandleLine(string line) {
      var detectSkip = DetectSkip();
      StartCoroutine(detectSkip);
      yield return bubbleTextHelper.ShowText(line);
      StopCoroutine(detectSkip);

      while (!IsValidDialogueProgression()) {
        yield return null;
      }
      yield return new WaitForEndOfFrame();
    }
    
    private IEnumerator DetectSkip() {
      while (true) {
        if (!playerInput.IsInteractDown()) {
          yield return null;
          continue;
        }

        bubbleTextHelper.SkipText();
        yield break;
      }
    }

    private bool IsValidDialogueProgression() {
      return playerInput.IsInteractDown();
    }
  }
}