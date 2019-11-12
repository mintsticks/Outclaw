using Outclaw.Heist;
using UnityEngine;

namespace Outclaw.City {
  public interface ObjectiveInteractable : ISenseElement {
    void InRange();
    void ExitRange();
    void Interact();
    bool HasInteraction();
    void EnableEffect();
    void DisableEffect();
    Transform ObjectiveTransform { get; }
    Bounds ObjectiveBounds { get; }
  }
}