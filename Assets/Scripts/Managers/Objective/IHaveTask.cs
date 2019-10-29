using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw{
  public interface IHaveTask
  {
    Task ContainedTask{ get; }
    Transform Location{ get; }
  }
}
