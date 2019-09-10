using UnityEngine;

namespace Outclaw.City {
  public interface Interactable {
    void InRange();
    void ExitRange();
    void Interact();
  }
}

