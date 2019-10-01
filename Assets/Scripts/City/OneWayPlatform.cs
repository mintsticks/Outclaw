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
      var angle = Vector2.Angle(player.PlayerVelocity, onDirection);
      Debug.Log(player.PlayerVelocity);
      if (angle < maxOnAngle) {
        collider.enabled = true;
        return;
      }

      collider.enabled = false;
    }
  }
}