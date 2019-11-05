using UnityEngine;

namespace Outclaw {
  [CreateAssetMenu(fileName = "Prompt Data", menuName = "Outclaw/Prompt Data")]
  public class PromptData : ScriptableObject {
    public Sprite promptImage;
    public string promptTitle;
    [TextArea(15,20)]
    public string defaultPromptDescription;
    [TextArea(15,20)]
    public string xboxDescription;
    public bool hasImage;
    public bool hasTitle;
  }
}