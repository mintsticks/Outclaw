using UnityEngine;

namespace City {
  public interface CityInteractable {
    void InRange();
    void ExitRange();
    void Interact();
    bool HasInteraction();
    void EnableEffect();
    void DisableEffect();
    SpriteRenderer GetSpriteRenderer();
  }
}