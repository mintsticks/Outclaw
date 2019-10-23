using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Outclaw {

  public interface ISpawnManager {

    string PreviousScene { get; set; }
    Vector3 GetSpawnPoint(string previousScene);

  }
  
  public class SpawnManager : IInitializable, ISpawnManager {

    private string previousScene;

    public string PreviousScene {
      get => previousScene;
      set => previousScene = value;
    }

    public void Initialize() {
      previousScene = "Start";
    }

    public Vector3 GetSpawnPoint(string previousScene) {
      
      return new Vector3();
    }
  }
}