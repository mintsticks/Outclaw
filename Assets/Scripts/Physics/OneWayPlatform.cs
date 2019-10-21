using System.Collections.Generic;
using UnityEngine;

namespace City {
  public class OneWayPlatform : MonoBehaviour {
    [SerializeField]
    private List<Collider2D> onColliderSet;

    [SerializeField]
    private List<Collider2D> offColliderSet;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private string layerWhenActivated;
    
    public void IntersectTrigger() {
      UpdateCollidersInSet(onColliderSet, true);
      UpdateCollidersInSet(offColliderSet, false);
      //TODO: find a better way to serialize the sorting layer instead of using strings
      spriteRenderer.sortingLayerName = layerWhenActivated;
    }

    private void UpdateCollidersInSet(List<Collider2D> colliders, bool enable) {
      foreach (var col in colliders) {
        col.enabled = enable;
      }
    }
  }
}