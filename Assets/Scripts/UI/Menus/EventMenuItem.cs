using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Outclaw.UI{
  public class EventMenuItem : AbstractMouseMenuItem
  {
    [SerializeField] private UnityEvent onSelect;

    public override void Select(){
      onSelect.Invoke();
    }
  }
}
