using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Outclaw.UI;
using Utility;

namespace Outclaw {
  public interface IPauseMenuManager { 
    bool Active { get; }
    void Unpause();
  }

  public class PauseMenuManager : Menu, IPauseMenuManager {
    
    [Header("Blur Effect")]
    [SerializeField] 
    protected float blurAmount;

    [SerializeField]
    private Image background;

    [Header("Pause Items")]
    [SerializeField]
    private PauseResumeItem pauseResumeItem;
    
    [SerializeField] 
    private EventMenuItem pauseInfoItem;
    
    [SerializeField]
    private PauseOptionItem pauseOptionItem;

    [SerializeField]
    private EventMenuItem pauseCreditsItem;

    [SerializeField]
    private PauseExitItem pauseExitItem;

    [SerializeField] private Submenu creditsSubmenu;

    [SerializeField] private Submenu optionsSubmenu;

    [SerializeField] private Submenu infoSubmenu;
    
    [Inject]
    private ISceneTransitionManager sceneTransitionManager;
    
    [Inject]
    private IPauseGame pause;

    private bool previouslyPaused;
    
    private List<IMenuItem> items;

    public bool Active { get => active; }

    private bool isSubmenuActive;

    protected override IMenuItem this[int i] { get => items[i]; }

    protected override int ItemCount() => items.Count;

    void Awake() {
      //Initialize list of pause items. Unity can't serialize interfaces, unfortunately.
      items = new List<IMenuItem> { pauseResumeItem, pauseInfoItem, pauseOptionItem, pauseCreditsItem, pauseExitItem };
      currentIndex = 0;
      items[0].Hover();
      contents.alpha = 0;

      isSubmenuActive = false;
      infoSubmenu.gameObject.SetActive(false);
      optionsSubmenu.gameObject.SetActive(false);
      creditsSubmenu.gameObject.SetActive(false);
    }
    
    void Update() {
      CheckActiveState();
      if (!active) {
        return;
      }

      if (isSubmenuActive)
      {
        return;
      }

      CheckSelectionState();
    }

    private void CheckActiveState() {
      if (!playerInput.IsPauseDown() || sceneTransitionManager.IsSwitching) {
        return;
      }

      if (active) {
        Unpause();
        return;
      } 
      Pause();
    }

    private void Pause() {
      previouslyPaused = pause.IsPaused;
      if(!previouslyPaused){
        pause.Pause();
      }
      StartCoroutine(FadeInContent());
      StartCoroutine(AnimateBlurIn());
      active = true;
    }
    
    private IEnumerator AnimateBlurIn() {
      for (var i = 0f; i < pauseTime; i += GlobalConstants.ANIMATION_FREQ) {
        background.material.SetFloat("_Radius", i * blurAmount / pauseTime);
        yield return new WaitForSecondsRealtime(GlobalConstants.ANIMATION_FREQ);
      }
    }
    
    public void Unpause() {
      StartCoroutine(AnimateBlurOut());
      StartCoroutine(FadeOutContent());
    }
    
    private IEnumerator AnimateBlurOut() {
      for (var i = 0f; i < pauseTime; i += GlobalConstants.ANIMATION_FREQ) {
        background.material.SetFloat("_Radius", blurAmount - i * blurAmount / pauseTime);
        yield return new WaitForSecondsRealtime(GlobalConstants.ANIMATION_FREQ);
      }
      if(!previouslyPaused){
        pause.Unpause();
      }
      active = false;
    }

    public void ActivateSubmenu(Submenu submenu) {
      submenu.gameObject.SetActive(true);
      submenu.Active = true;
      isSubmenuActive = true;
    }
    
    public void ReturnFromSubmenu(Submenu submenu) {
      submenu.gameObject.SetActive(false);
      submenu.Active = false;
      isSubmenuActive = false;
    }
  }
}