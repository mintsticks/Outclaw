using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Outclaw.City {
  [Serializable]
  public class DialogueIconInfo {
    public string type;
    public Sprite icon;
  }

  public interface IDialogueIconManager {
    Sprite FindIconForString(string type);
  }
  
  public class DialogueIconManager : MonoBehaviour, IDialogueIconManager {
    [SerializeField]
    private List<DialogueIconInfo> iconInfos;
    
    public Sprite FindIconForString(string type) {
      return (from info in iconInfos where info.type == type select info.icon).FirstOrDefault();
    }
  }
}