using System;
using UnityEngine;

namespace Utility {
  public class InputStringHelper {
    public static string GetStringForInput(InputType type) {
      if (Application.platform == RuntimePlatform.XboxOne) {
        return GetXboxInputString(type);
      }

      return GetDefaultInputString(type);
    }

    private static string GetDefaultInputString(InputType type) {
      switch (type) {
        case InputType.INTERACT:
          return "Z";
        case InputType.SENSE:
          return "X";
        case InputType.JUMP:
          return "SPC";
        case InputType.DESCEND:
          return "↓";
        case InputType.LEFT:
          return "←";
        case InputType.RIGHT:
          return "→";
        case InputType.SNEAK:
          return "C";
        case InputType.START:
          return "space";
      }
      return "Z";
    }
    
    private static string GetXboxInputString(InputType type) {
      switch (type) {
        case InputType.INTERACT:
          return "B";
        case InputType.SENSE:
          return "Y";
        case InputType.JUMP:
          return "A";
        case InputType.DESCEND:
          return "↓";
        case InputType.LEFT:
          return "←";
        case InputType.RIGHT:
          return "→";
        case InputType.SNEAK:
          return "RB";
        case InputType.START:
          return "start";
      }
      return "X";
    }
  }

  public enum InputType {
    NONE = 0,
    INTERACT = 1,
    SENSE = 2,
    JUMP = 3,
    DESCEND = 4,
    LEFT = 5,
    RIGHT = 6,
    SNEAK = 7,
    START = 8
  }
}