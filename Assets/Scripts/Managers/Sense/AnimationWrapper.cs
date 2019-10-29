using System.Collections;
using UnityEngine;

namespace Outclaw {
  public class AnimationWrapper : MonoBehaviour {
    private IEnumerator currentSenseAnimation;
    public void StopCurrentAnimation() {
      if (currentSenseAnimation == null) {
        return;
      }
      StopCoroutine(currentSenseAnimation);
    }

    public void StartNewAnimation(IEnumerator animation) {
      StopCurrentAnimation();
      currentSenseAnimation = animation;
      StartCoroutine(currentSenseAnimation);
    }
  }
}