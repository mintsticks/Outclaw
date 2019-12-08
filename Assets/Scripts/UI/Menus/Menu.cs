using System;
using System.Collections;
using System.Collections.Generic;
using Outclaw.ManagedRoutine;
using UnityEngine;
using Utility;
using Zenject;

namespace Outclaw.UI{
  public abstract class Menu : MonoBehaviour
  {
    [Header("Audio")]
    [SerializeField] protected AudioClip moveSound;
    [SerializeField] protected AudioClip selectSound;
    [SerializeField] protected AudioClip showMenuSound;

    [Header("Fading Content")]
    [SerializeField] protected float pauseTime;
    [SerializeField] protected CanvasGroup contents;

    protected const int NO_INDEX = -1;
    protected int currentIndex = NO_INDEX;

    protected bool active = false;

    protected float waitTime = 0.25f;

    protected ManagedCoroutine upWait = null;

    protected ManagedCoroutine downWait = null;

    [Inject] protected IPlayerInput playerInput;
    [Inject] protected ISoundManager soundManager;

    protected abstract IMenuItem this[int i]{ get; }
    protected abstract int ItemCount();
    public bool Selectable => currentIndex != NO_INDEX;

    protected void Start() {
      upWait = new ManagedCoroutine(this, StallInput);
      downWait = new ManagedCoroutine(this, StallInput);

      for(int i = 0; i < ItemCount(); ++i){
        if(this[i] is AbstractMouseMenuItem){
          ((AbstractMouseMenuItem)this[i]).InitMenu(this);
        }
      }
    }

    protected virtual void CheckSelectionState() {
      if(currentIndex == NO_INDEX){
        return;
      }

      CheckDownSelection();
      CheckUpSelection();
      CheckItemSelect();
    }

    protected virtual void CheckItemSelect() {
      if (!playerInput.IsInteractDown()) {
        return;
      }
      
      soundManager.PlaySFX(selectSound);
      this[currentIndex].Select();
    }
    
    protected virtual void CheckDownSelection() {
      if (!playerInput.IsDownPress() || downWait.IsRunning) {
        return;
      }
      
      downWait.StartCoroutine();
      if (upWait.IsRunning) {
        upWait.StopCoroutine();
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
      if (!playerInput.IsUpPress() || upWait.IsRunning) {
        return;
      }
      
      upWait.StartCoroutine();
      if (downWait.IsRunning) {
        downWait.StopCoroutine();
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
      soundManager.PlaySFX(moveSound);
    }

    protected IEnumerator FadeInContent() {
      currentIndex = NO_INDEX;
      for (var i = 0f; i < pauseTime; i += GlobalConstants.ANIMATION_FREQ) {
        contents.alpha = i / pauseTime;
        yield return new WaitForSecondsRealtime(GlobalConstants.ANIMATION_FREQ);
      }

      currentIndex = 0;
      this[0].Hover();
      contents.alpha = 1;
    }
    
    protected IEnumerator FadeOutContent() {
      for (var i = pauseTime; i >= 0; i -= GlobalConstants.ANIMATION_FREQ) {
        contents.alpha = i / pauseTime;
        yield return new WaitForSecondsRealtime(GlobalConstants.ANIMATION_FREQ);
      }

      contents.alpha = 0;
    }

    protected IEnumerator StallInput() {
      yield return new WaitForSecondsRealtime(waitTime);
    }

    public void SelectItem(IMenuItem item){
      // find item 
      int newIdx = 0;
      while(this[newIdx] != item){
        newIdx++;
      }

      if(newIdx >= ItemCount()){
        Debug.LogWarning(item + " is not part of this menu.");
        return;
      }

      HoverIndex(currentIndex, newIdx);
      currentIndex = newIdx;
    }
  }
}
