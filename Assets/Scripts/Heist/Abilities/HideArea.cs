#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
namespace Outclaw.Heist{
  public class HideArea : MonoBehaviour, Interactable
  {
    [SerializeField] private City.Indicator hideIndicator;
    [SerializeField] private AudioClip hideSound;

    [Inject] private IHideablePlayer hidePlayer;
    [Inject] private City.IPlayer player;
    [Inject] private ISoundManager soundManager;

    public void Awake() {
      hideIndicator.Initialize(transform);
    }

    public void InRange() {
      hideIndicator.CreateIndicator();
      StartCoroutine(hideIndicator.FadeIn());
    }

    public void ExitRange() {
      StartCoroutine(hideIndicator.FadeOut());
    }

    public void Interact(){
      player.PlayerTransform.position = transform.position;
      hidePlayer.Hidden = !hidePlayer.Hidden;
      soundManager.PlaySFX(hideSound);
    }
  }
}
