using System.Collections;
using System.Collections.Generic;
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
      public List<Bounds> InvalidBounds;
      public CatDialogueUI UI;
      public Vector3? InitialPosition;
    }
    
    [SerializeField] private RectTransform bubbleImageTransform;
    [SerializeField] private Image bubbleImage;

    [SerializeField] private BubbleAnimationHelper bubbleAnimationHelper;
    [SerializeField] private BubbleTail bubbleTail;
    [SerializeField] private BubbleTextHelper bubbleTextHelper;
    [SerializeField] private BubblePositionHelper bubblePositionHelper;
    [Inject] private IDialogueSettings dialogueSettings;

    [Inject]
    public void Initialize(Data data) {
      bubbleImage.color = dialogueSettings.BubbleColor;
      bubbleTail.SetColor(dialogueSettings.BubbleColor);
      transform.SetParent(data.UIParent, false);
      var invalidBounds = data.InvalidBounds ?? new List<Bounds>();
      var canvas = data.UI.DialogueCanvas;
      InitializeBubblePosition(data, canvas, invalidBounds);
      bubbleTextHelper.Initialize(canvas, bubbleImageTransform, (int)dialogueSettings.FontSize, data.BubbleText);
    }

    private void InitializeBubblePosition(Data data, Canvas canvas, List<Bounds> invalidBounds) {
      if (data.InitialPosition == null) {
        bubblePositionHelper.Initialize(invalidBounds, Camera.main, data.BubbleParent, bubbleImageTransform, canvas);
        return;
      }
      bubblePositionHelper.Initialize(data.InitialPosition.Value, data.BubbleParent, Camera.main);
    }

    public Transform BubbleTransform => transform;
    
    public IEnumerator FadeBubble() {
      bubblePositionHelper.StopFollowing();
      yield return bubbleAnimationHelper.FadeBubble();
      Destroy(gameObject);
    }

    public IEnumerator ShowText(string text) {
      yield return bubbleTextHelper.ShowText(text);
    }
    
    public void SkipText() {
      bubbleTextHelper.SkipText();
    }
  }

  public enum DialogueType {
    NONE,
    THOUGHT,
    SPEECH
  }
}