using System;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw {
  [CreateAssetMenu(fileName = "New Platform Info", menuName = "Outclaw/PlatformInfo")]
  public class PlatformInfo : ScriptableObject {
    public List<PlatformText> texts;
    public List<PlatformImage> images;
  }
  
  [Serializable]
  public class PlatformText {
    public RuntimePlatform platform;
    public string text;
  }
  
  [Serializable]
  public class PlatformImage {
    public RuntimePlatform platform;
    public Sprite image;
  }
}