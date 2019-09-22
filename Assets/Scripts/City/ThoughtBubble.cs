using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Outclaw.City {
  public class ThoughtBubble : MonoBehaviour {
    public class Factory : PlaceholderFactory<Data, ThoughtBubble> { }

    public class Data {
      public List<string> Options;
      public Action<int> OnSelect;
    }

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private Text bubbleText;

    [SerializeField]
    private Image bubble;

    [SerializeField]
    private Transform trail;
    
    [SerializeField]
    private Transform indicatorGrid;

    [SerializeField]
    private Transform leftArrow;

    [SerializeField]
    private Transform rightArrow;
    
    [Inject]
    private IPlayer player;

    [Inject]
    private IPlayerInput playerInput;
    
    [Inject]
    private OptionIndicator.Factory optionIndicatorFactory;

    private List<string> options;
    private List<OptionIndicator> indicators;
    private int currentIndex;
    private Action<int> onSelect;

    [Inject]
    public void Initialize(Data data) {
      options = data.Options;
      onSelect = data.OnSelect;
      currentIndex = 0;
      indicators = new List<OptionIndicator>();
      for (var i = 0; i < options.Count; i++) {
        var indicator = optionIndicatorFactory.Create();
        indicator.transform.SetParent(indicatorGrid);
        indicators.Add(indicator);
      }
      
      SetOption(currentIndex);
      SetPosition();
    }

    private void Update() {
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

    private void SetPosition() {
      var mainCam = Camera.main;
      if (mainCam == null) {
        return;
      }
      transform.position = mainCam.WorldToScreenPoint(player.PlayerTransform.position + offset);
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
      //TODO(downg): add some animation between options
      UpdateArrows(index);
      bubbleText.text = options[index];
      indicators[currentIndex].Deselect();
      indicators[index].Select();
      currentIndex = index;
    }

    private void UpdateArrows(int index) {
      leftArrow.gameObject.SetActive(index > 0);
      rightArrow.gameObject.SetActive(index < options.Count - 1);
    }

    public void SetOpacity(float opacity) {
      var oldTextColor = bubbleText.color;
      bubbleText.color = new Color(oldTextColor.r, oldTextColor.g, oldTextColor.b, opacity);
      
      var oldBubbleColor = bubble.color;
      bubble.color =  new Color(oldBubbleColor.r, oldBubbleColor.g, oldBubbleColor.b, opacity);
    }

    public void ToBubble() {
      trail.gameObject.SetActive(false);
      indicatorGrid.gameObject.SetActive(false);
      leftArrow.gameObject.SetActive(false);
      rightArrow.gameObject.SetActive(false);
    }
  }
}