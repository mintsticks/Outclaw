using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ModestTree;
using UI.Dialogue;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Outclaw.City {
  public interface Bubble {
    Transform BubbleTransform { get; }
  }

  public class ThoughtBubble : MonoBehaviour, Bubble {
    public class Factory : PlaceholderFactory<Data, ThoughtBubble> { }

    public class Data {
      public List<string> Options;
      public Action<int> OnSelect;
      public string BubbleText;
      public Transform BubbleParent;
      public Transform UIParent;
      public List<Bounds> InvalidBounds;
      public CatDialogueUI UI;
      public Vector3? InitialPosition;
    }
    
    [SerializeField] private Transform indicatorGrid;
    [SerializeField] private Transform leftArrow;
    [SerializeField] private Transform rightArrow;
    
    [SerializeField] private Image bubble;
    [SerializeField] private RectTransform bubbleImageTransform;
    [SerializeField] private BubbleTail bubbleTail;
    [SerializeField] private BubbleTextHelper bubbleTextHelper;
    [SerializeField] private BubblePositionHelper bubblePositionHelper;
    [SerializeField] private BubbleAnimationHelper bubbleAnimationHelper;
    [SerializeField] private AnimationWrapper animationWrapper;

    [SerializeField] private float optionPadding = 15f;
    
    [Inject] private IPlayerInput playerInput;
    [Inject] private OptionIndicator.Factory optionIndicatorFactory;
    [Inject] private IDialogueSettings dialogueSettings;
    [Inject] private IPauseGame pause;

    private List<string> options;
    private List<OptionIndicator> indicators;
    private int currentIndex;
    private Action<int> onSelect;
    private bool initialized;

    [Inject]
    public void Initialize(Data data) {
      bubble.color = dialogueSettings.BubbleColor;
      bubbleTail.SetColor(dialogueSettings.BubbleColor);
      transform.SetParent(data.UIParent, false);
      var invalidBounds = data.InvalidBounds ?? new List<Bounds>();
      var canvas = data.UI.DialogueCanvas;
      InitializeBubblePosition(data, canvas, invalidBounds);
      
      
      options = data.Options;
      onSelect = data.OnSelect;
      currentIndex = 0;
      if (options.Count <= 1) {
        indicatorGrid.gameObject.SetActive(false);
        bubbleTextHelper.Initialize(canvas, bubbleImageTransform, (int)dialogueSettings.FontSize, data.BubbleText, 0);
        return;
      }
      
      bubbleTextHelper.Initialize(canvas, bubbleImageTransform, (int)dialogueSettings.FontSize, data.BubbleText, optionPadding);
      indicators = new List<OptionIndicator>();
      for (var i = 0; i < options.Count; i++) {
        var indicator = optionIndicatorFactory.Create();
        indicator.transform.SetParent(indicatorGrid);
        indicators.Add(indicator);
      }
    }
    
    private void InitializeBubblePosition(Data data, Canvas canvas, List<Bounds> invalidBounds) {
      if (data.InitialPosition == null) {
        bubblePositionHelper.Initialize(invalidBounds, Camera.main, data.BubbleParent, bubbleImageTransform, canvas);
        return;
      }
      bubblePositionHelper.Initialize(data.InitialPosition.Value, data.BubbleParent, Camera.main);
    }

    public Transform BubbleTransform => transform;

    private void Update() {
      if (pause.IsPaused) {
        return;
      }

      if (!initialized) {
        SetOption(0);
        initialized = true;
      }
      
      if (playerInput.IsLeftDown()) {
        SelectLeft();
      }

      if (playerInput.IsRightDown()) {
        SelectRight();
      }

      if (playerInput.IsInteractDown()) {
        Select();
      }
    }


    private void SelectLeft() {
      if (currentIndex <= 0) {
        return;
      }
      SetOption(currentIndex - 1);
    }

    private void SelectRight() {
      if (currentIndex >= options.Count - 1) {
        return;
      }
      SetOption(currentIndex + 1);
    }

    private void Select() {
      onSelect.Invoke(currentIndex);
    }

    private void SetOption(int index) {
      UpdateArrows(index);
      UpdateText(options[index].Trim());
      if (options.Count <= 1) {
        return;
      }

      indicators[currentIndex].Deselect();
      indicators[index].Select();
      currentIndex = index;
    }

    private void UpdateText(string text) {
      animationWrapper.StartNewAnimation(bubbleTextHelper.ShowText(text));
    }
    
    private void UpdateArrows(int index) {
      leftArrow.gameObject.SetActive(index > 0);
      rightArrow.gameObject.SetActive(index < options.Count - 1);
    }

    public IEnumerator FadeBubble() {
      yield return bubbleAnimationHelper.FadeBubble();
      Destroy(gameObject);
    }
  }
}