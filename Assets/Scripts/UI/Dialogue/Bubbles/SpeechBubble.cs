﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UI.Dialogue;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Outclaw.City {
  public class SpeechBubble : MonoBehaviour, Bubble {
    public class Factory : PlaceholderFactory<Data, SpeechBubble> { }

    public class Data {
      public string BubbleText;
      public Transform BubbleParent;
      public Transform UIParent;
      public DialogueType Type;
      public List<Bounds> InvalidBounds;
      public CatDialogueUI UI;
    }
    
    [SerializeField] private float bubbleFadeTime;
    [SerializeField] private AnimationCurve bubbleFade;

    [SerializeField] private float textSpeed = 0.025f;
    [SerializeField] private float characterAnimateSpeed = 1000f;
    [SerializeField] private bool isUpperCase = true;
    [SerializeField] private int startFontSize = 1;
    [SerializeField] private float horizontalPadding = 20f;
    [SerializeField] private float verticalPadding = 15f;

    [SerializeField] private Text bubbleText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform bubbleImage;
    [SerializeField] private BubblePositionHelper bubblePositionHelper;
    [SerializeField] private BubbleTail bubbleTail;
    
    private bool skipped;
    private Transform parent;
    private Camera main;
    private StringBuilder currentStringBuilder;
    private List<Bounds> invalidBounds;
    private Vector3 parentCachedPos;
    private Canvas canvas;
    
    [Inject]
    public void Initialize(Data data) {
      bubbleText.text = data.BubbleText;
      parent = data.BubbleParent;
      main = Camera.main;
      HandleType(data.Type);
      transform.SetParent(data.UIParent, false);
      invalidBounds = data.InvalidBounds ?? new List<Bounds>();
      canvas = data.UI.DialogueCanvas;
      bubblePositionHelper.Initialize(invalidBounds, main, parent, bubbleImage, canvas);
    }

    public IEnumerator FadeBubble() {
      bubblePositionHelper.StopFollowing();
      for (var t = 0f; t <= bubbleFadeTime; t += Time.deltaTime) {
        SetOpacity(1 - bubbleFade.Evaluate(t / bubbleFadeTime));
        bubbleTail.SetOpacity(1 - bubbleFade.Evaluate(t / bubbleFadeTime));
        yield return null;
      }
      Destroy(gameObject);
    }
    
    private string ProcessText(string text) {
      bubbleText.color = bubbleText.color.WithAlpha(0);
      var newText = TestText(text);
      CheckTextBounds(newText);
      bubbleText.text = "";
      bubbleText.color = bubbleText.color.WithAlpha(1f);
      return newText;
    }
    
    private string TestText(string text) {
      if (isUpperCase) {
        text = text.ToUpper();
      }

      var processed = new StringBuilder();
      var buffer = new StringBuilder();
      var previousHeight = -1f;
      
      foreach (var word in text.Split(' ')) {
        buffer.Append(word + " ");
        bubbleText.text = buffer.ToString();
        var height = LayoutUtility.GetPreferredHeight(bubbleText.rectTransform);
        if (previousHeight < 0f) {
          previousHeight = height;
        }

        if (Math.Abs(height - previousHeight) > .001) {
          previousHeight = height;
          processed.Append("\n");
        }

        processed.Append(word + " ");
      }
      
      return processed.ToString().TrimEnd();
    }

    private void CheckTextBounds(string text) {
      bubbleText.text = text;
      var height = LayoutUtility.GetPreferredHeight(bubbleText.rectTransform);
      var width = LayoutUtility.GetPreferredWidth(bubbleText.rectTransform);
      if (width < bubbleText.rectTransform.sizeDelta.x) {
        bubbleText.rectTransform.sizeDelta = new Vector2(width, height);
      }
      
      bubbleImage.sizeDelta = new Vector2(width + horizontalPadding * 2, height + verticalPadding * 2);
      bubbleText.rectTransform.position = bubbleText.rectTransform.position.AddToXY(canvas.scaleFactor * horizontalPadding, canvas.scaleFactor * -verticalPadding);
      bubbleText.color = bubbleText.color.WithAlpha(1f);
    }

    public IEnumerator ShowText(string text) {
      text = ProcessText(text);
      currentStringBuilder = new StringBuilder();
      skipped = false;
      
      foreach (var c in text) {
        if (skipped) {
          bubbleText.text = text;
          skipped = false;
          yield return new WaitForEndOfFrame();
          yield break;
        }

        var size = startFontSize;
        currentStringBuilder.Append("<size=" + size + ">" + c + "</size>");
        currentStringBuilder = new StringBuilder(UpdateCharacters(currentStringBuilder.ToString()));
        yield return new WaitForSeconds(textSpeed);
        bubbleText.text = currentStringBuilder.ToString();
      }
      StartCoroutine(UpdateCharacterRoutine());
    }

    private IEnumerator UpdateCharacterRoutine() {
      while (true) {
        var currentString = currentStringBuilder.ToString();
        var updatedString = UpdateCharacters(currentString);
        if (currentString == updatedString) {
          bubbleText.text = RemoveTags(updatedString);
          yield break;
        }

        currentStringBuilder = new StringBuilder(updatedString);
        bubbleText.text = updatedString;
        yield return null;
      }
    }

    private string RemoveTags(string text) {
      text = Regex.Replace(text, "<size=\\d+>", "");
      return Regex.Replace(text, "</size>", "");
    }
    
    private string UpdateCharacters(string text) {
      var toIncrease = (int) (Time.deltaTime * characterAnimateSpeed);
      return Regex.Replace(text, "\\d+", m => {
        var size = Mathf.Min(int.Parse(m.Value) + toIncrease, bubbleText.fontSize);
        return size.ToString();
      });
    }
    
    public void SkipText() {
      skipped = true;
    }
    
    public Transform BubbleTransform => transform;
    
    public void SetOpacity(float opacity) {
      canvasGroup.alpha = opacity;
    }

    private void HandleType(DialogueType type) {
      /*var isSpeech = type == DialogueType.SPEECH;
      var isThought = type == DialogueType.THOUGHT;
      
      speechTrail.gameObject.SetActive(isSpeech);
      thoughtTrail.gameObject.SetActive(isThought);*/
    }
  }

  public enum DialogueType {
    NONE,
    THOUGHT,
    SPEECH
  }
}