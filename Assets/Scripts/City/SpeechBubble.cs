using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Outclaw.City {
  public class SpeechBubble : MonoBehaviour {
    public class Factory : PlaceholderFactory<Data, SpeechBubble> { }

    public class Data {
      public string BubbleText;
      public Transform BubbleParent;
    }

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private Text bubbleText;

    [SerializeField]
    private Image tail;

    [SerializeField]
    private Image bubble;
    
    [Inject]
    public void Initialize(Data data) {
      bubbleText.text = data.BubbleText;

      var mainCam = Camera.main;
      if (mainCam == null) {
        return;
      }
      transform.position = mainCam.WorldToScreenPoint(data.BubbleParent.position + offset);
    }

    public void SetText(string text) {
      bubbleText.text = text;
    }

    public void SetOpacity(float opacity) {
      var oldTextColor = bubbleText.color;
      bubbleText.color = new Color(oldTextColor.r, oldTextColor.g, oldTextColor.b, opacity);
      
      var oldBubbleColor = bubble.color;
      bubble.color =  new Color(oldBubbleColor.r, oldBubbleColor.g, oldBubbleColor.b, opacity);
    }

    public void RemoveTail() {
      tail.gameObject.SetActive(false);
    }
  }
}