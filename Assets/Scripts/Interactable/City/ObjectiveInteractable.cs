using UnityEngine;

namespace Outclaw.City {
  public interface ObjectiveInteractable {
    void InRange();
    void ExitRange();
    void Interact();
    bool HasInteraction();
    void EnableEffect();
    void DisableEffect();
    SpriteRenderer GetSpriteRenderer();
  }
}