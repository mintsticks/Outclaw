using UnityEngine;

namespace Outclaw {
  [CreateAssetMenu(fileName = "New Icon Name Data", menuName = "Outclaw/Dialogue/IconName")]
  public class IconNameData : ScriptableObject {
    public string name;
    public Sprite icon;
  }
}