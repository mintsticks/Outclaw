using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Zenject;

namespace Outclaw {

  public interface ISpawnManager {

    string PreviousScene { get; set; }
    Vector3? GetSpawnPoint();
    Vector3? GetCheckpoint();

    string LastCheckpoint { get; set; }
    List<Checkpoint> Checkpoints { get; }
    void ClearCheckpoints();
  }
  
  public class SpawnManager : MonoBehaviour, IInitializable, ISpawnManager {

    private string previousScene;
    private string lastCheckpoint;
    private List<Checkpoint> checkpoints;

    public string PreviousScene {
      get => previousScene;
      set => previousScene = value;
    }

    public string LastCheckpoint
    {
      get => lastCheckpoint;
      set => lastCheckpoint = value;
    }

    public List<Checkpoint> Checkpoints => checkpoints;

    private void Awake() {
      checkpoints = new List<Checkpoint>();
    }

    public void Initialize() {
      previousScene = "Start";
      lastCheckpoint = "Start";
    }

    public Vector3? GetSpawnPoint() {
      List<SpawnPoint> spawnList;
      spawnList = GameObject.Find("SpawnList")?.GetComponent<SpawnList>()?.SpawnPoints;
      if(spawnList == null){
        return null;
      }

      var entryPoint = spawnList.FirstOrDefault(point => point.EntryLocation.SceneName.Equals(previousScene));
      if (entryPoint == null) {
        return null;
      }
      return entryPoint.PointPosition;
    }

    public Vector3? GetCheckpoint() {
      var checkpoint = checkpoints.FirstOrDefault(point => point.CheckpointName.Equals(lastCheckpoint));
      if (checkpoint == null) {
        return null;
      }
      return checkpoint.transform.position;
    }
    
    public void ClearCheckpoints(){
      checkpoints.Clear();
    }
  }
}