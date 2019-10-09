using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Outclaw.UI{
  public abstract class Menu : MonoBehaviour
  {
    [Header("Fading Content")]
    [SerializeField] protected float pauseTime;
    [SerializeField] protected float animationFreq = .02f;
    [SerializeField] protected CanvasGroup contents;

    protected int currentIndex;

    protected bool active = false;

    [Inject]
    protected IPlayerInput playerInput;

    protected abstract IMenuItem this[int i]{ get; }
    protected abstract int ItemCount();

    protected virtual void CheckSelectionState() {
      CheckDownSelection();
      CheckUpSelection();
      CheckItemSelect();
    }

    protected virtual void CheckItemSelect() {
      if (!playerInput.IsInteractDown()) {
        return;
      }
      
      this[currentIndex].Select();
    }
    
    protected virtual void CheckDownSelection() {
      if (!playerInput.IsDownPress()) {
        return;
      }
      
      if (currentIndex >= ItemCount() - 1) {
        HoverIndex(ItemCount() - 1, 0);
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
        HoverIndex(0, ItemCount() - 1);
        currentIndex = ItemCount() - 1;
        return;
      }
      
      HoverIndex(currentIndex, currentIndex - 1);
      currentIndex--;
    }

    protected virtual void HoverIndex(int oldIndex, int newIndex) {
      this[oldIndex].Unhover();
      this[newIndex].Hover();
    }

    protected IEnumerator FadeInContent() {
      for (var i = 0f; i < pauseTime; i += animationFreq) {
        contents.alpha = i / pauseTime;
        yield return new WaitForSecondsRealtime(animationFreq);
      }
    }
    
    protected IEnumerator FadeOutContent() {
      for (var i = pauseTime; i >= 0; i -= animationFreq) {
        contents.alpha = i / pauseTime;
        yield return new WaitForSecondsRealtime(animationFreq);
      }
    }
  }
}
