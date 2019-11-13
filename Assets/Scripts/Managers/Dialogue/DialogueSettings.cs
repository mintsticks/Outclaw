using UnityEngine;

namespace Outclaw {
  public interface IDialogueSettings {
    float FontSize { get; }
    Color BubbleColor { get; }
  }

  public class DialogueSettings : MonoBehaviour, IDialogueSettings {
    [SerializeField] private float fontSize;
    [SerializeField] private Color bubbleColor;
    public float FontSize => fontSize;
    public Color BubbleColor => bubbleColor;
  }
}