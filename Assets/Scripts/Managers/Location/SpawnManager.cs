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
    Vector3 GetSpawnPoint();

  }
  
  public class SpawnManager : MonoBehaviour, IInitializable, ISpawnManager {

    private string previousScene;

    public string PreviousScene {
      get => previousScene;
      set => previousScene = value;
    }

    public void Initialize() {
      previousScene = "Start";
    }

    public Vector3 GetSpawnPoint() {
      List<SpawnPoint> spawnList;
      try {
        spawnList = GameObject.Find("SpawnList").GetComponent<SpawnList>().SpawnPoints;
      }
      catch (NullReferenceException e) {
        return new Vector3(-1, -1, -1);
      }

      var entryPoint = spawnList.FirstOrDefault(point => point.EntryScene.Equals(previousScene));
      if (entryPoint == null) {
        return new Vector3(-1, -1, -1);
      }
      return entryPoint.PointPosition;
    }
  }
}