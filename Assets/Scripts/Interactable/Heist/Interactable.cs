using UnityEngine;

namespace Outclaw {
  public enum InteractableState{
    Invisible,
    DisabledVisible,
    Enabled
  }

  public interface Interactable {
    void InRange(InteractableState state);
    void ExitRange();
    void Interact();
  }
}

