using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Outclaw.City {
  public class SpeechBubble : MonoBehaviour, Bubble {
    public class Factory : PlaceholderFactory<Data, SpeechBubble> { }

    public class Data {
      public string BubbleText;
      public Transform BubbleParent;
      public DialogueType Type;
    }

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private Text bubbleText;

    [SerializeField]
    private Transform speechTrail;

    [SerializeField]
    private Transform thoughtTrail;
    
    [SerializeField]
    private CanvasGroup canvas;

    private Transform tail;
    private Transform parent;
    private Camera main;
    
    [Inject]
    public void Initialize(Data data) {
      bubbleText.text = data.BubbleText;
      parent = data.BubbleParent;
      main = Camera.main;
      transform.position = main.WorldToScreenPoint(parent.position + offset);
      HandleType(data.Type);
    }

    private void Update() {
      transform.position = main.WorldToScreenPoint(parent.position + offset);
    }

    public Transform BubbleTransform => transform;
    
    public void SetText(string text) {
      bubbleText.text = text;
    }

    public void SetOpacity(float opacity) {
      canvas.alpha = opacity;
    }

    public void RemoveTail() {
      tail.gameObject.SetActive(false);
    }

    private void HandleType(DialogueType type) {
      var isSpeech = type == DialogueType.SPEECH;
      var isThought = type == DialogueType.THOUGHT;
      
      tail = isSpeech ? speechTrail : thoughtTrail;
      speechTrail.gameObject.SetActive(isSpeech);
      thoughtTrail.gameObject.SetActive(isThought);
    }
  }
  
  public enum DialogueType {
    NONE,
    THOUGHT,
    SPEECH
  }
}