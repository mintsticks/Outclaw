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
    public Platform platform;
    public string text;
  }
  
  [Serializable]
  public class PlatformImage {
    public Platform platform;
    public Sprite image;
  }
  
  public enum Platform {
    NONE = 0,
    PC = 1,
    XBOX = 2
  }
}