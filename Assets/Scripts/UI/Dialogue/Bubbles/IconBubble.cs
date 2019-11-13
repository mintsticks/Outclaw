using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Managers.Dialogue;
using UI.Dialogue;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Outclaw.City {
  public class IconBubble : MonoBehaviour, Bubble {
    public class Factory : PlaceholderFactory<Data, IconBubble> { }

    public class Data {
      public string IconName;
      public Transform BubbleParent;
      public Transform UIParent;
      public List<Bounds> InvalidBounds;
      public CatDialogueUI UI;
    }
    
    [SerializeField] private float bubbleFadeTime;
    [SerializeField] private AnimationCurve bubbleFade;

    [SerializeField] private Image bubbleImage;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform bubbleTransform;
    [SerializeField] private BubblePositionHelper bubblePositionHelper;
    [SerializeField] private BubbleTail bubbleTail;

    [Inject] private IIconNameManager iconNameManager;

    [Inject]
    public void Initialize(Data data) {
      Debug.Log(data.IconName);
      bubbleImage.sprite = iconNameManager.IconForName(data.IconName);
      transform.SetParent(data.UIParent, false);
      var parent = data.BubbleParent;
      var invalidBounds = data.InvalidBounds ?? new List<Bounds>();
      bubblePositionHelper.Initialize(invalidBounds, Camera.main, parent, bubbleTransform, data.UI.DialogueCanvas);
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

    public Transform BubbleTransform => transform;
    
    public void SetOpacity(float opacity) {
      canvasGroup.alpha = opacity;
    }
  }
}