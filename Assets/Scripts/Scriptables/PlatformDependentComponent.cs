using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Outclaw {
  [CreateAssetMenu(fileName = "New Platform Component", menuName = "Outclaw/PlatformComponent")]
  public class PlatformDependentComponent : ScriptableObject {
    public List<PlatformEntry> platformData;
  }
  
  [Serializable]
  public class PlatformEntry {
    public Platform platform;
    public GameObject prefab;
  }
}