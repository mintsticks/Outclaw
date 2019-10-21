using System.Collections;
using System.Collections.Generic;
using Outclaw.City;
using UnityEngine;
using Zenject;

namespace Outclaw{
  public interface IPauseGame{
    bool IsPaused { get; }
    void Pause();
    void Unpause();
  }

  public class PauseGame : MonoBehaviour, IPauseGame {
    private bool isPaused;

    public bool IsPaused { get => isPaused; }

    public void Pause(){
      isPaused = true;
      Time.timeScale = 0f;
    }

    public void Unpause(){
      isPaused = false;
      Time.timeScale = 1f;
    }
  }
}
