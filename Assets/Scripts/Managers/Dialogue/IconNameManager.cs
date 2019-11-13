using System.Collections.Generic;
using System.Linq;
using Outclaw;
using UnityEngine;

namespace Managers.Dialogue {
  public interface IIconNameManager {
    Sprite IconForName(string name);
  }
  
  public class IconNameManager : MonoBehaviour, IIconNameManager {
    [SerializeField] private List<IconNameData> icons;

    public Sprite IconForName(string key) {
      return icons.FirstOrDefault(data => data.name == key)?.icon;
    }
  }
}