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

    private Canvas canvas;
    private List<Bounds> invalidBounds;
    private List<string> options;
    private string bubbleText;
    private Transform bubbleParent;
    private Vector3? initialPosition;
    
    private List<OptionIndicator> indicators;
    private int currentIndex;
    private Action<int> onSelect;
    private bool initialized;

    private IEnumerator detectSkip;

    [Inject]
    public void Initialize(Data data) {
      invalidBounds = data.InvalidBounds ?? new List<Bounds>();
      canvas = data.UI.DialogueCanvas;
      bubbleText = data.BubbleText;
      transform.SetParent(data.UIParent, false);
      options = data.Options;
      onSelect = data.OnSelect;
      bubbleParent = data.BubbleParent;
      initialPosition = data.InitialPosition;

      bubble.color = dialogueSettings.BubbleColor;
      bubbleTail.SetColor(dialogueSettings.BubbleColor);
      currentIndex = 0;
      if (options.Count <= 1) {
        indicatorGrid.gameObject.SetActive(false);
        return;
      }
      
      indicators = new List<OptionIndicator>();
      for (var i = 0; i < options.Count; i++) {
        var indicator = optionIndicatorFactory.Create();
        indicator.transform.SetParent(indicatorGrid, false);
        indicators.Add(indicator);
      }
    }

    public void SetupBubble() {
      var padding = options.Count <= 1 ? 0 : optionPadding;
      bubbleTextHelper.Initialize(canvas, bubbleImageTransform, (int)dialogueSettings.FontSize, bubbleText, padding);
      InitializeBubblePosition();
    }
    
    private void InitializeBubblePosition() {
      bubblePositionHelper.Initialize(invalidBounds, Camera.main, bubbleParent, bubbleImageTransform, canvas, initialPosition);
    }

    public Transform BubbleTransform => transform;

    private void Update() {
      if (pause.IsPaused) {
        return;
      }

      if (!initialized) {
        SetOption(0);
        initialized = true;
        return;
      }
      
      if (playerInput.IsLeftDown()) {
        SelectLeft();
      }

      if (playerInput.IsRightDown()) {
        SelectRight();
      }

      if (!bubbleTextHelper.IsRunningText && playerInput.IsInteractDown()) {
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
      animationWrapper.StartNewAnimation(RunText(text), StopDialogue);
    }
    
    private void UpdateArrows(int index) {
      leftArrow.gameObject.SetActive(index > 0);
      rightArrow.gameObject.SetActive(index < options.Count - 1);
    }

    public IEnumerator FadeBubble() {
      yield return bubbleAnimationHelper.FadeBubble();
      Destroy(gameObject);
    }
    
    private void StopDialogue(){
      StopCoroutine(detectSkip);
      bubbleTextHelper.StoppedDialogue();
    }

    private IEnumerator RunText(string text){
      detectSkip = bubbleTextHelper.DetectSkip();
      StartCoroutine(detectSkip);
      yield return bubbleTextHelper.ShowText(text);
      StopDialogue();
    }
  }
}