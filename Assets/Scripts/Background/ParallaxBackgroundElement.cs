using System.Collections.Generic;
using Outclaw;
using UnityEngine;

namespace Background {
  public class ParallaxBackgroundElement : MonoBehaviour {
    [Range(0, 1), SerializeField] private float parallaxAmount;
    [SerializeField] private List<SpriteRenderer> sprites;
    
    private Camera cam;
    private Vector3 cachedCameraPos;
    
    private void Start() {
      cam = Camera.main;
    }

    private void Update() {
      foreach (var spriteRenderer in sprites) {
        UpdateSprite(spriteRenderer);
      }
      cachedCameraPos = cam.transform.position;
    }
    
    private void UpdateSprite(SpriteRenderer sprite) {
      var dist = cam.transform.position.x - cachedCameraPos.x;
      var currentPos = sprite.transform.position;
      sprite.transform.position = currentPos.WithX(parallaxAmount * dist + currentPos.x);
    }
  }
}