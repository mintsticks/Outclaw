using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Outclaw.City {
  public interface IPauseMenuManager {
    bool IsPaused { get; }
    void Unpause();
  }

  public class PauseMenuManager : MonoBehaviour, IPauseMenuManager {
    [SerializeField]
    private float pauseTime;

    [SerializeField]
    private float blurAmount;

    [SerializeField]
    private float animationFreq = .02f;
    
    [SerializeField]
    private CanvasGroup contents;
    
    [SerializeField]
    private Image background;

    [Header("Pause Items")]
    [SerializeField]
    private PauseResumeItem pauseResumeItem;
    
    [SerializeField]
    private PauseSaveItem pauseSaveItem;
    
    [SerializeField]
    private PauseLoadItem pauseLoadItem;
    
    [SerializeField]
    private PauseOptionItem pauseOptionItem;
    
    [SerializeField]
    private PauseExitItem pauseExitItem;
    
    [Inject]
    private IPlayerInput playerInput;

    [Inject]
    private ISceneTransitionManager sceneTransitionManager;
    
    private List<IPauseItem> pauseItems;
    private int currentIndex;
    private bool isPaused;
    public bool IsPaused => isPaused;

    void Awake() {
      //Initialize list of pause items. Unity can't serialize interfaces, unfortunately.
      pauseItems = new List<IPauseItem> { pauseResumeItem, pauseSaveItem, pauseLoadItem, pauseOptionItem, pauseExitItem};
      currentIndex = 0;
      pauseItems[0].Hover();
      isPaused = false;
      contents.alpha = 0;
    }
    
    void Update() {
      CheckPauseState();
      if (!isPaused) {
        return;
      }

      CheckSelectionState();
    }

    private void CheckSelectionState() {
      CheckDownSelection();
      CheckUpSelection();
      CheckItemSelect();
    }

    private void CheckItemSelect() {
      if (!playerInput.IsInteractDown()) {
        return;
      }
      
      pauseItems[currentIndex].Select();
    }
    
    private void CheckDownSelection() {
      if (!playerInput.IsDownPress()) {
        return;
      }
      
      if (currentIndex >= pauseItems.Count - 1) {
        HoverIndex(pauseItems.Count - 1, 0);
        currentIndex = 0;
        return;
      }
      
      HoverIndex(currentIndex, currentIndex + 1);
      currentIndex++;
    }
    
    private void CheckUpSelection() {
      if (!playerInput.IsUpPress()) {
        return;
      }
      
      if (currentIndex <= 0) {
        HoverIndex(0, pauseItems.Count - 1);
        currentIndex = pauseItems.Count - 1;
        return;
      }
      
      HoverIndex(currentIndex, currentIndex - 1);
      currentIndex--;
    }

    private void HoverIndex(int oldIndex, int newIndex) {
      pauseItems[oldIndex].Unhover();
      pauseItems[newIndex].Hover();
    }
    
    private void CheckPauseState() {
      if (!playerInput.IsPauseDown() || sceneTransitionManager.IsSwitching) {
        return;
      }

      if (isPaused) {
        Unpause();
        return;
      } 
      Pause();
    }

    private void Pause() {
      isPaused = true;
      StartCoroutine(FadeInContent());
      StartCoroutine(AnimateBlurIn());
      Time.timeScale = 0.0f;
    }

    private IEnumerator FadeInContent() {
      for (var i = 0f; i < pauseTime; i += animationFreq) {
        contents.alpha = i / pauseTime;
        yield return new WaitForSecondsRealtime(animationFreq);
      }
    }
    
    private IEnumerator FadeOutContent() {
      for (var i = pauseTime; i >= 0; i -= animationFreq) {
        contents.alpha = i / pauseTime;
        yield return new WaitForSecondsRealtime(animationFreq);
      }
    }
    
    private IEnumerator AnimateBlurIn() {
      for (var i = 0f; i < pauseTime; i += animationFreq) {
        background.material.SetFloat("_Radius", i * blurAmount / pauseTime);
        yield return new WaitForSecondsRealtime(animationFreq);
      }
    }
    
    public void Unpause() {
      StartCoroutine(AnimateBlurOut());
      StartCoroutine(FadeOutContent());
    }
    
    private IEnumerator AnimateBlurOut() {
      for (var i = 0f; i < pauseTime; i += animationFreq) {
        background.material.SetFloat("_Radius", blurAmount - i * blurAmount / pauseTime);
        yield return new WaitForSecondsRealtime(animationFreq);
      }
      Time.timeScale = 1.0f;
      isPaused = false;
    }
  }
}