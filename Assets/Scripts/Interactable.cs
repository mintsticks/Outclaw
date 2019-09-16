using UnityEngine;

namespace Outclaw {
  public interface Interactable {
    void InRange();
    void ExitRange();
    void Interact();
  }
}

