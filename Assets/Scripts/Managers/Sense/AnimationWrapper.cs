using System;
using System.Collections;
using UnityEngine;

namespace Outclaw {
  public class AnimationWrapper : MonoBehaviour {
    private IEnumerator currentSenseAnimation;
    private Action stopCallback;

    public void StopCurrentAnimation() {
      if (currentSenseAnimation == null) {
        return;
      }
      StopCoroutine(currentSenseAnimation);
      stopCallback?.Invoke();
      Reset();
    }

    public void StartNewAnimation(IEnumerator animation, Action stopCallback = null) {
      StopCurrentAnimation();
      currentSenseAnimation = animation;
      this.stopCallback = stopCallback;
      StartCoroutine(ResetOnComplete(currentSenseAnimation));
    }

    private IEnumerator ResetOnComplete(IEnumerator enumerator){
      yield return enumerator;
      Reset();
    }

    private void Reset(){
      currentSenseAnimation = null;
      stopCallback = null;
    }
  }
}