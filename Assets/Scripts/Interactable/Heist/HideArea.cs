#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
namespace Outclaw.Heist{
  public class HideArea : MonoBehaviour, Interactable {
    [SerializeField] private Indicator hideIndicator;
    [SerializeField] private AudioClip hideSound;
    [SerializeField] private Task promptTask;
    
    [Inject] private IHideablePlayer hidePlayer;
    [Inject] private City.IPlayer player;
    [Inject] private ISoundManager soundManager;

    public void InRange() {
      hideIndicator.FadeIn();
    }

    public void ExitRange() {
      hideIndicator.FadeOut();
    }

    public void Interact() {
      if (promptTask != null && !promptTask.IsComplete) {
        promptTask.Complete();
      }
      player.PlayerTransform.position = transform.position;
      soundManager.PlaySFX(hideSound);
      if (!hidePlayer.Hidden) {
        hideIndicator.FadeOut();
        hidePlayer.Hidden = true;
        return;
      }
      hideIndicator.FadeIn();
      hidePlayer.Hidden = false;
    }
  }
}
