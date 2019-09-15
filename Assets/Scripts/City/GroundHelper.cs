using System;
using UnityEngine;

namespace Outclaw.City {
  public class GroundHelper : MonoBehaviour {
    [SerializeField]
    private Collider2D collider;

    [SerializeField]
    private LayerMask groundLayer;
    
    public bool IsGrounded() {
      var epsilon = .1f;
      var colliderBounds = collider.bounds;
      var boxSize = new Vector2((colliderBounds.max.x - colliderBounds.min.x) / 2, epsilon);
      var boxPos = new Vector2(transform.position.x, colliderBounds.min.y - epsilon / 2);
      return Physics2D.OverlapBox(boxPos, boxSize, 0, groundLayer);
    }
  }
}