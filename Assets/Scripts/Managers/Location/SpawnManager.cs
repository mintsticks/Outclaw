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
    string LastCheckpoint { get; set; }
    Vector3 GetSpawnPoint();
    Vector3 GetCheckpoint();
    List<Checkpoint> Checkpoints { get; }
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

    public Vector3 GetSpawnPoint() {
      List<SpawnPoint> spawnList;
      try {
        spawnList = GameObject.Find("SpawnList").GetComponent<SpawnList>().SpawnPoints;
      }
      catch (NullReferenceException e) {
        return new Vector3(-1, -1, -1);
      }

      var entryPoint = spawnList.FirstOrDefault(point => point.EntryLocation.SceneName.Equals(previousScene));
      if (entryPoint == null) {
        return new Vector3(-1, -1, -1);
      }
      return entryPoint.PointPosition;
    }

    public Vector3 GetCheckpoint() {
      var checkpoint = checkpoints.FirstOrDefault(point => point.CheckpointName.Equals(lastCheckpoint));
      if (checkpoint == null) {
        return new Vector3(-1,-1,-1);
      }
      return checkpoint.transform.position;
    }
    
  }
}