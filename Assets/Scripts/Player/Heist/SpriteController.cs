using System.Collections;
using Managers;
using Outclaw.City;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public class SpriteController : MonoBehaviour {
    [SerializeField] private SpriteBundle sprites;
    [SerializeField] private Color regularColor;
    [SerializeField] private Color sneakColor;
    [SerializeField] private float transitionTime;
    [Inject] private IPlayer player;
    [Inject] private ISneakManager sneakManager;

    private IEnumerator currentAnimation;

    private void Awake() {
      sprites.SetColor(regularColor);
    }

    public void UpdateColor() {
      if (player.InputDisabled) {
        return;
      }

      if (sneakManager.IsSneakingDown) {
        StopCurrentAnimation();
        StartAnimation(TransitionColor(sneakColor));
        return;
      }

      if (sneakManager.IsSneakingUp) {
        StopCurrentAnimation();
        StartAnimation(TransitionColor(regularColor));
      }
    }

    private void StopCurrentAnimation() {
      if (currentAnimation != null) {
        StopCoroutine(currentAnimation);
      }
    }

    private void StartAnimation(IEnumerator anim) {
      currentAnimation = anim;
      StartCoroutine(anim);
    }

    private IEnumerator TransitionColor(Color to) {
      var from = sprites.GetColor();
      for (var i = 0f; i <= transitionTime; i += Time.deltaTime) {
        sprites.SetColor(Color.Lerp(from, to, i / transitionTime));
        yield return null;
      }
    }
  }
}