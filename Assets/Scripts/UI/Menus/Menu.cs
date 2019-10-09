using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Outclaw.UI{
  public abstract class Menu : MonoBehaviour
  {
    protected List<IMenuItem> items;
    protected int currentIndex;

    protected bool active = false;

    [Inject]
    protected IPlayerInput playerInput;

    protected virtual void CheckSelectionState() {
      CheckDownSelection();
      CheckUpSelection();
      CheckItemSelect();
    }

    protected virtual void CheckItemSelect() {
      if (!playerInput.IsInteractDown()) {
        return;
      }
      
      items[currentIndex].Select();
    }
    
    protected virtual void CheckDownSelection() {
      if (!playerInput.IsDownPress()) {
        return;
      }
      
      if (currentIndex >= items.Count - 1) {
        HoverIndex(items.Count - 1, 0);
        currentIndex = 0;
        return;
      }
      
      HoverIndex(currentIndex, currentIndex + 1);
      currentIndex++;
    }
    
    protected virtual void CheckUpSelection() {
      if (!playerInput.IsUpPress()) {
        return;
      }
      
      if (currentIndex <= 0) {
        HoverIndex(0, items.Count - 1);
        currentIndex = items.Count - 1;
        return;
      }
      
      HoverIndex(currentIndex, currentIndex - 1);
      currentIndex--;
    }

    protected virtual void HoverIndex(int oldIndex, int newIndex) {
      items[oldIndex].Unhover();
      items[newIndex].Hover();
    }
  }
}
