using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Outclaw.UI;

namespace Outclaw{
  public class EventMenu : Menu
  {
    [SerializeField] private EventMenuItem[] eventItems;

    protected override IMenuItem this[int i] { get => eventItems[i]; }

    protected override int ItemCount() => eventItems.Length;

    void Awake(){
      SetInteractable(false);
    }

    void Update() {
      if (active) {
        CheckSelectionState();
      }
    }

    public void Show(){
      active = true;
      StartCoroutine(FadeInContent());
    }

    public void Hide(){
      active = false;
      StartCoroutine(FadeOutContent());
    }
  }
}
