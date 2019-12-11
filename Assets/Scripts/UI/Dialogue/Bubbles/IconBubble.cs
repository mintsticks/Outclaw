using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private Image bubbleImage;
    [SerializeField] private RectTransform bubbleTransform;
    [SerializeField] private BubblePositionHelper bubblePositionHelper;
    [SerializeField] private BubbleAnimationHelper bubbleAnimationHelper;
    
    [Inject] private IIconNameManager iconNameManager;

    [Inject]
    public void Initialize(Data data) {
      bubbleImage.sprite = iconNameManager.IconForName(data.IconName);
      transform.SetParent(data.UIParent, false);
      var parent = data.BubbleParent;
      var invalidBounds = data.InvalidBounds ?? new List<Bounds>();
      bubblePositionHelper.Initialize(invalidBounds, Camera.main, parent, bubbleTransform, data.UI.DialogueCanvas);
    }

    public IEnumerator FadeBubble() {
      yield return bubbleAnimationHelper.FadeBubble();
      Destroy(gameObject);
    }

    public Transform BubbleTransform => transform;
  }
}