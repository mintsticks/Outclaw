using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Outclaw.UI;

namespace Outclaw.Heist{
  public class CapturedMenu : Menu, ICapturedMenu
  {
    [SerializeField] private EventMenuItem[] eventItems;

    [Inject] private IPauseGame pause;
    [Inject] private IPauseMenuManager pauseMenu;

    protected override IMenuItem this[int i] { get => eventItems[i]; }

    protected override int ItemCount() => eventItems.Length;

    void Awake(){
      this[0].Hover();
      contents.alpha = 0;
    }

    void Update() {
      if (active && !pauseMenu.Active) {
        CheckSelectionState();
      }
    }

    void OnDestroy(){
      pause.Unpause();
    }

    public void Show(){
      pause.Pause();
      active = true;
      StartCoroutine(FadeInContent());
    }

    public void Hide(){
      pause.Unpause();
      active = false;
      StartCoroutine(FadeOutContent());
    }
  }
}
