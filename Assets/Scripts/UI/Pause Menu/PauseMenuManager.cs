using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Outclaw.UI;

namespace Outclaw {
  public interface IPauseMenuManager {
    bool Active { get; }
    void Unpause();
  }

  public class PauseMenuManager : Menu, IPauseMenuManager {
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
    private ISceneTransitionManager sceneTransitionManager;
    
    [Inject]
    private IPauseGame pause;

    public bool Active { get => active; }

    void Awake() {
      //Initialize list of pause items. Unity can't serialize interfaces, unfortunately.
      items = new List<IMenuItem> { pauseResumeItem, pauseSaveItem, pauseLoadItem, pauseOptionItem, pauseExitItem};
      currentIndex = 0;
      items[0].Hover();
      contents.alpha = 0;
    }
    
    void Update() {
      CheckActiveState();
      if (!active) {
        return;
      }

      CheckSelectionState();
    }

    private void CheckActiveState() {
      if (!playerInput.IsPauseDown() || sceneTransitionManager.IsSwitching) {
        return;
      }

      if (pause.IsPaused) {
        Unpause();
        return;
      } 
      Pause();
    }

    private void Pause() {
      pause.Pause();
      StartCoroutine(FadeInContent());
      StartCoroutine(AnimateBlurIn());
      active = true;
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
      pause.Unpause();
      active = false;
    }
  }
}