using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Outclaw.UI;

namespace Outclaw.Heist{
  public class CapturedMenu : Menu, ICapturedMenu
  {
    [SerializeField] private EventMenuItem[] eventItems;
    [SerializeField] private CanvasGroup contents;

    [Inject] private IPauseGame pause;
    [Inject] private IPauseMenuManager pauseMenu;

    void Awake(){
      items = new List<IMenuItem>(eventItems);
      Hide();
    }

    void Update() {
      if (active && !pauseMenu.Active) {
        CheckSelectionState();
      }
    }

    public void Show(){
      pause.Pause();
      active = true;
      contents.alpha = 1;
    }

    public void Hide(){
      pause.Unpause();
      active = false;
      contents.alpha = 0;
    }
  }
}
