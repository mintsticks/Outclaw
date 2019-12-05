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
    [SerializeField] private AnimationWrapper animationWrapper;
    
    [Inject] private IPlayer player;
    [Inject] private ISneakManager sneakManager;
    [Inject] private IPlayerLitManager litManager;
    
    private bool movingToSneakColor;
    
    private void Awake() {
      sprites.SetColor(regularColor);
    }

    public void UpdateColor() {
      if (player.InputDisabled) {
        return;
      }

      if (!movingToSneakColor && sneakManager.IsSneaking && !litManager.IsLit) {
        animationWrapper.StartNewAnimation(TransitionColor(sneakColor));
        movingToSneakColor = true;
        return;
      }

      if (movingToSneakColor && (!sneakManager.IsSneaking || litManager.IsLit)) {
        animationWrapper.StartNewAnimation(TransitionColor(regularColor));
        movingToSneakColor = false;
      }
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