using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Outclaw.UI;
using Utility;

namespace Outclaw {
  public class CurrentObjectiveDisplay : MonoBehaviour {

    [Inject] private IGameStateManager gameStateManager;

    [SerializeField] private Text objectiveText;

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
      objectiveText.text = gameStateData.ObjectiveText;
    }
  }
}