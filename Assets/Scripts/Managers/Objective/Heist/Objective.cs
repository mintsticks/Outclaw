using System.Collections;
using System.Collections.Generic;
using Outclaw.City;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public class Objective : MonoBehaviour, Interactable {
    [SerializeField]
    private Indicator objectiveIndicator;

    [Inject]
    private IObjectiveManager objectiveManager;
    
    public bool IsComplete {
      get => isComplete;
    }

    private bool isComplete;

    public void Awake() {
      objectiveManager.AddObjective(this);
    }

    public void InRange(InteractableState state) {
      switch(state){
        case InteractableState.DisabledVisible:
          objectiveIndicator.FadeToDisabled();
          break;
        case InteractableState.Enabled:
          objectiveIndicator.FadeIn();
          break;
      }
    }

    public void ExitRange() {
      objectiveIndicator.FadeOut();
    }

    public void Interact() {
      CompleteObjective();
    }
    
    private void CompleteObjective() {
      isComplete = true;
    }
  }
}