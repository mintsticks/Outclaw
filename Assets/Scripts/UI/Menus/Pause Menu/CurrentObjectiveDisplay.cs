using System.Collections;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Outclaw.UI;
using Utility;

namespace Outclaw {
  public class CurrentObjectiveDisplay : MonoBehaviour {

    [Inject] private IGameStateManager gameStateManager;

    [SerializeField] private Text objectiveText;
    [SerializeField] private string defaultText = "Explore the world!";
    
    private GameStateData gameStateData;

    void Awake() {
      gameStateData = gameStateManager.CurrentGameStateData;
      UpdateDisplay();
    }

    void Update() {
      if (gameStateData == gameStateManager.CurrentGameStateData) {
        return;
      }
      gameStateData = gameStateManager.CurrentGameStateData;
      UpdateDisplay();
    }

    private void UpdateDisplay() {
      if (gameStateData == null) {
        return;
      }

      if (gameStateData.ObjectiveText.IsEmpty()) {
        objectiveText.text = defaultText;
        return;
      }
      
      objectiveText.text = gameStateData.ObjectiveText;
    }
  }
}