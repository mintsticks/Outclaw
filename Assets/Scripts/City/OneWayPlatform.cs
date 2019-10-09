using System;
using System.Collections.Generic;
using Outclaw;
using Outclaw.City;
using UnityEngine;
using Zenject;

namespace City {
  public class OneWayPlatform : MonoBehaviour {
    [SerializeField]
    private List<Collider2D> onColliderSet;

    [SerializeField]
    private List<Collider2D> offColliderSet;
    
    [SerializeField]
    private Collider2D trigger;

    [SerializeField]
    private Vector2 onDirection;

    [Inject]
    private IPlayer player;

    public void IntersectTrigger() {
      var playerDir = player.PlayerTransform.position - trigger.transform.position;
      var inOnDirection = Vector2.Angle(playerDir, onDirection) < 90;
      Debug.Log(Vector2.Angle(playerDir, onDirection));
      UpdateCollidersInSet(onColliderSet, inOnDirection);
      UpdateCollidersInSet(offColliderSet, !inOnDirection);
    }

    private void UpdateCollidersInSet(List<Collider2D> colliders, bool enable) {
      foreach (var col in colliders) {
        col.enabled = enable;
      }
    }
  }
}