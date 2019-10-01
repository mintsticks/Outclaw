using System;
using Outclaw;
using Outclaw.City;
using UnityEngine;
using Zenject;

namespace City {
  public class OneWayPlatform : MonoBehaviour {
    [SerializeField]
    private Collider2D collider;

    [SerializeField]
    private Vector2 onDirection;

    [SerializeField]
    private float maxOnAngle;
    
    [Inject]
    private IPlayerInput playerInput;

    [Inject]
    private IPlayer player;

    private void Update() {
      if (Math.Abs(player.PlayerVelocity.magnitude) < 0.001f) {
        collider.enabled = true;
        return;
      }
      
      var angle = Vector2.Angle(player.PlayerVelocity, onDirection);
      if (angle < maxOnAngle) {
        collider.enabled = true;
        return;
      }

      collider.enabled = false;
    }
  }
}