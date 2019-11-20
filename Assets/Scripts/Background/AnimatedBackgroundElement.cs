using System.Collections.Generic;
using UnityEngine;

namespace Outclaw {
  public class AnimatedBackgroundElement : MonoBehaviour {
    [Range(0, 10), SerializeField] private float movementSpeed = 4;
    [SerializeField] private List<SpriteRenderer> sprites;
    [SerializeField] private bool movingLeft;
    
    private void Update() {
      foreach (var spriteRenderer in sprites) {
        UpdateSprite(spriteRenderer);
      }
    }

    private void UpdateSprite(SpriteRenderer sprite) {
      var currentPos = sprite.transform.position;
      var moveLeftPos = currentPos.WithX(currentPos.x - movementSpeed * Time.deltaTime);
      var moveRightPos = currentPos.WithX(currentPos.x + movementSpeed * Time.deltaTime);
      sprite.transform.position = movingLeft ? moveLeftPos : moveRightPos;
    }
  }
}