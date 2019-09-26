using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Outclaw.City {
  public interface ISceneTransitionManager {
    void TransitionToScene(string scene);
    bool IsSwitching { get; }
  }
  
  public class SceneTransitionManager : MonoBehaviour, ISceneTransitionManager {
    [SerializeField]
    private Image fadeImage;

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
      StartCoroutine(DelayTransition());
      yield return loadingOp;
      
      yield return FadeOut();
      isSwitching = false;
    }

    private IEnumerator DelayTransition() {
      loadingOp.allowSceneActivation = false;
      yield return new WaitForSeconds(waitTime);
      loadingOp.allowSceneActivation = true;
    }
    
    private IEnumerator FadeIn() {
      for (var i = 0f; i <= fadeTime; i += Time.deltaTime) {
        var color = fadeImage.color;
        fadeImage.color = new Color(color.r, color.g, color.b,i / fadeTime);
        yield return null;
      }
    }

    private IEnumerator FadeOut() {
      for (var i = fadeTime; i >= 0; i -= Time.deltaTime) {
        var color = fadeImage.color;
        fadeImage.color = new Color(color.r, color.g, color.b,i / fadeTime);
        yield return null;
      }
    }
  }
}