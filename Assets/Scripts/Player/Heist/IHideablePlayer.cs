using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Outclaw.Heist{
  public interface IHideablePlayer{
    bool Hidden { get; set; }
    Transform PlayerTransform { get; }
  } 
}
