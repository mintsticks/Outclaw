using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Outclaw.City {
  public interface IPauseMenuManager {
    bool IsPaused { get; }
  }

  public class PauseMenuManager : MonoBehaviour, IPauseMenuManager {
    [SerializeField]
    private Canvas canvas;

    [Inject]
    private IPlayerInput playerInput;
    
    private bool isPaused;
    public bool IsPaused => isPaused;

    void Start() {
      isPaused = false;
      canvas.gameObject.SetActive(false);
    }

    private void Pause() {
      isPaused = true;
      canvas.gameObject.SetActive(true);
      Time.timeScale = 0.0f;
    }

    private void Unpause() {
      isPaused = false;
      canvas.gameObject.SetActive(false);
      Time.timeScale = 1.0f;
    }
    
    void Update() {
      if (!playerInput.IsPauseDown()) {
        return;
      }

      if (isPaused) {
        Unpause();
        return;
      } 
      Pause();
    }
  }
}