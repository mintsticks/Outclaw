using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Outclaw {
  public interface ISceneTransitionManager {
    void TransitionToScene(string scene);
    bool IsSwitching { get; }
  }
  
  public class SceneTransitionManager : MonoBehaviour, ISceneTransitionManager {
    [SerializeField]
    private CanvasGroup content;

    [SerializeField]
    private float fadeTime;

    [SerializeField]
    private float waitTime;
    
    private bool isSwitching;
    private AsyncOperation loadingOp;

    public bool IsSwitching => isSwitching;

    public void TransitionToScene(string scene) {
      if (isSwitching) {
        return;
      }
      isSwitching = true;
      StartCoroutine(TransitionRoutine(scene));
    }
    
    private IEnumerator TransitionRoutine(string scene) {
      yield return FadeIn();
      
      loadingOp = SceneManager.LoadSceneAsync(scene);
      yield return loadingOp;
      
      yield return FadeOut();
      isSwitching = false;
    }
    
    
    private IEnumerator FadeIn() {
      for (var i = 0f; i <= fadeTime; i += Time.unscaledDeltaTime) {
        content.alpha = i / fadeTime;
        yield return null;
      }

      content.alpha = 1f;
    }

    private IEnumerator FadeOut() {
      for (var i = fadeTime; i >= 0; i -= Time.unscaledDeltaTime) {
        content.alpha = i / fadeTime;
        yield return null;
      }

      content.alpha = 0f;
    }
  }
}