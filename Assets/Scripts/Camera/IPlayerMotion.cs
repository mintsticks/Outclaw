using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw{
  public interface IPlayerMotion
  {
    Transform PlayerTransform { get; }
    Vector3 Velocity { get; }
    bool IsGrounded { get; }
    bool IsFacingLeft { get; }
  }
}
