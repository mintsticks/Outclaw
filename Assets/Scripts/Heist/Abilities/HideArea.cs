#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
namespace Outclaw.Heist{
  public class HideArea : MonoBehaviour, Interactable
  {
    [SerializeField] private City.Indicator hideIndicator;
    private bool isHiding = false;

    [Inject] IHideablePlayer hidePlayer;
    [Inject] City.IPlayer player;

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
    }
  }
}
