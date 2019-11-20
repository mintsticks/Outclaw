using Outclaw;
using UnityEngine;

namespace DefaultNamespace {
  public class RepeatedBackgroundElement : MonoBehaviour {
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private SpriteRenderer spriteCopy;

    private Camera cam;
    private SpriteRenderer currentSprite;
    private SpriteRenderer otherSprite;

    private bool currentSpriteOnLeft;
    private void Start() {
      cam = Camera.main;
      currentSprite = sprite;
      otherSprite = spriteCopy;
      if (currentSprite.transform.position.x < otherSprite.transform.position.x) {
        currentSpriteOnLeft = true;
      }
    }

    private void Update() {
      CheckExitLeft();
      CheckExitRight();
      UpdateSpritePositions();
    }

    private void UpdateSpritePositions() {
      var camX = cam.transform.position.x;
      var currentX = currentSprite.transform.position.x;
      if (camX > currentX && !currentSpriteOnLeft) {
        otherSprite.transform.position = otherSprite.transform.position.WithX(currentSprite.bounds.max.x + otherSprite.bounds.extents.x);
        currentSpriteOnLeft = true;
        return;
      }
      
      if (camX < currentX && currentSpriteOnLeft) {
        otherSprite.transform.position = otherSprite.transform.position.WithX(currentSprite.bounds.min.x - otherSprite.bounds.extents.x);
        currentSpriteOnLeft = false;
      }
    }

    private void CheckExitLeft() {
      if (!(cam.transform.position.x < currentSprite.bounds.min.x)) {
        return;
      }
      
      SwapSprites();
      currentSpriteOnLeft = true;
    }
    
    private void CheckExitRight() {
      if (!(cam.transform.position.x > currentSprite.bounds.max.x)) {
        return;
      }
      
      SwapSprites();
      currentSpriteOnLeft = false;
    }
    
    private void SwapSprites() {
      var temp = currentSprite;
      currentSprite = otherSprite;
      otherSprite = temp;
    }
  }
}