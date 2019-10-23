using System.Collections.Generic;
using Outclaw.City;

namespace Outclaw {
  public class InteractableEffectManager {
    private List<ObjectiveInteractable> interactables = new List<ObjectiveInteractable>();
    private HashSet<ObjectiveInteractable> currentInteractablesToEffect;
    
    public void UpdateInteractables() {
      currentInteractablesToEffect = new HashSet<ObjectiveInteractable>();
      foreach (var interactable in interactables) {
        if (interactable.HasInteraction()) {
          interactable.EnableEffect();
          continue;
        } 
        currentInteractablesToEffect.Add(interactable);
      }
    }

    public void RegisterInteractable(ObjectiveInteractable interactable) {
      interactables.Add(interactable);
    }
  }
}