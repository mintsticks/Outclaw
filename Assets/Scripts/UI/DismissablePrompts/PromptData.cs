using UnityEngine;

namespace Outclaw {
  [CreateAssetMenu(fileName = "Prompt Data", menuName = "Outclaw/Prompt Data")]
  public class PromptData : ScriptableObject {
    public Sprite promptImage;
    public string promptTitle;
    public string promptDescription;
    public bool hasImage;
    public bool hasTitle;
  }
}