using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using ModestTree;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Outclaw {
  public class BubbleTextHelper : MonoBehaviour {
    [Header("Text Speed")]
    [SerializeField] private float textSpeed = 0.025f;
    [SerializeField] private float characterAnimateSpeed = 1000f;

    [Header("Text Appearance")]
    [SerializeField] private bool isUpperCase = true;
    [SerializeField] private int startFontSize = 1;
    [SerializeField] private float horizontalPadding = 20f;
    [SerializeField] private float verticalPadding = 15f;
    [SerializeField] private float defaultWidth = 200f;

    [SerializeField] private Text bubbleText;
    
    [Header("Audio")]
    [SerializeField] private AudioClip textScrollClip;
    [Inject] private ISoundManager soundManager;

    [Inject] private IPlayerInput playerInput;
    
    private bool skipped;
    private float bottomPadding;
    private StringBuilder currentStringBuilder;
    private RectTransform bubbleImageTransform;
    private bool isRunningText;

    public bool IsRunningText => isRunningText;
    
    public void Initialize(Canvas canvas, RectTransform bubbleImageTransform, int fontSize, string initialText = "", float bottomPadding = 0f) {
      this.bubbleImageTransform = bubbleImageTransform;
      bubbleText.text = initialText;
      bubbleText.fontSize = fontSize;
      bubbleText.rectTransform.position = 
        bubbleText.rectTransform.position.AddToXY(
          canvas.scaleFactor * horizontalPadding, 
          canvas.scaleFactor * -verticalPadding);
      this.bottomPadding = bottomPadding;

      if (initialText == null || initialText.IsEmpty()) {
        return;
      }
      ProcessText(initialText);
    }
    
    private string ProcessText(string text) {
      bubbleText.color = bubbleText.color.WithAlpha(0);
      bubbleText.rectTransform.sizeDelta = new Vector2(defaultWidth, 0);
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
      if (bubbleImageTransform != null) {
        bubbleImageTransform.sizeDelta =
          new Vector2(width + horizontalPadding * 2, height + verticalPadding * 2 + bottomPadding);
      }
      bubbleText.color = bubbleText.color.WithAlpha(1f);
    }

    public IEnumerator ShowText(string text) {
      isRunningText = true;
      text = ProcessText(text);

      soundManager.PlaySFX(textScrollClip);
      currentStringBuilder = new StringBuilder();
      skipped = false;
      
      foreach (var c in text) {
        if (skipped) {
          bubbleText.text = text;
          skipped = false;
          yield return new WaitForEndOfFrame();
          soundManager.StopSFX();
          isRunningText = false;
          yield break;
        }

        var size = startFontSize;
        currentStringBuilder.Append("<size=" + size + ">" + c + "</size>");
        currentStringBuilder = new StringBuilder(UpdateCharacters(currentStringBuilder.ToString()));
        yield return new WaitForSeconds(textSpeed);
        bubbleText.text = currentStringBuilder.ToString();
      }
      StartCoroutine(UpdateCharacterRoutine());
      soundManager.StopSFX();
      isRunningText = false;
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
      return Regex.Replace(text, "<size=\\d+>", tag => {
        return Regex.Replace(tag.Value, "\\d+", m => {
            var size = Mathf.Min(int.Parse(m.Value) + toIncrease, bubbleText.fontSize);
            return size.ToString();
          });
      });
    }
    
    public void SkipText() {
      skipped = true;
    }

    public IEnumerator DetectSkip() {
      // wait 1 frame before trying to check for skip because immediately
      //   when this starts is the same frame as the InteractDown event
      yield return null;
      
      while (true) {
        if (!playerInput.IsInteractDown()) {
          yield return null;
          continue;
        }

        SkipText();
        yield break;
      }
    }
  }
}