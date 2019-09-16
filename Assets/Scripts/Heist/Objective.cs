using System.Collections;
using System.Collections.Generic;
using Outclaw.City;
using UnityEngine;

namespace Outclaw.Heist {
  public class Objective : MonoBehaviour, Interactable {
    [SerializeField]
    private Indicator objectiveIndicator;

    private bool isComplete;

    public void Awake() {
      objectiveIndicator.Initialize(transform);
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