using Outclaw.Heist;
using UnityEngine;

namespace Outclaw.City {
  public interface ObjectiveInteractable : ISenseElement, Interactable {
    bool HasInteraction();
    void EnableEffect();
    void DisableEffect();
    Transform ObjectiveTransform { get; }
    Bounds ObjectiveBounds { get; }
  }
}