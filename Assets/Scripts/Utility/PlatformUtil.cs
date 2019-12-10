namespace Utility {
  public static class PlatformUtil {
    public static Platform GetPlatform() {
#if UNITY_WSA
      return Platform.XBOX_ONE;
#else
      //DEFAULT PLATFORM IS PC
      return Platform.PC;
#endif
    }
  }

  public enum Platform {
    NONE = 0,
    XBOX_ONE = 1,
    PC = 2,
  }
}