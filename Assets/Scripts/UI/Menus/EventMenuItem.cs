using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Outclaw.UI{
  public class EventMenuItem : MonoBehaviour, IMenuItem
  {
    [SerializeField] private MenuItemText itemText;
    [SerializeField] private UnityEvent onSelect;

    public void Select(){
      onSelect.Invoke();
    }

    public void Hover() {
      itemText.Hover();
    }

    public void Unhover() {
      itemText.Unhover();
    }
  }
}
