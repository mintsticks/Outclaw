using Outclaw.City;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public class Vent : MonoBehaviour, Interactable {
    [SerializeField]
    private Vector3 ventOffset;
    
    [SerializeField]
    private Vent destination;

    [SerializeField]
    private Indicator ventIndicator;
    
    [SerializeField]
    private AudioClip ventSound;
    
    [Inject]
    private IPlayer player;

    [Inject]
    private ISoundManager soundManager;
    
    public void InRange() {
      ventIndicator.FadeIn();
    }

    public void ExitRange() {
      ventIndicator.FadeOut();
    }

    public void Interact() {
      player.PlayerTransform.position = destination.transform.position + ventOffset;
      soundManager.PlaySFX(ventSound);
    }
  }
}