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
      objectiveIndicator.Initialize(transform);
      objectiveManager.AddObjective(this);
    }
    
    public void InRange() {
      objectiveIndicator.CreateIndicator();
      StartCoroutine(objectiveIndicator.FadeIn());
    }

    public void ExitRange() {
      StartCoroutine(objectiveIndicator.FadeOut());
    }

    public void Interact() {
      CompleteObjective();
    }
    
    private void CompleteObjective() {
      isComplete = true;
      objectiveIndicator.SpriteColor = Color.green;
    }
  }
}