using System;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw {
  public class SpawnList : MonoBehaviour {
    private List<SpawnPoint> spawnPoints;

    public List<SpawnPoint> SpawnPoints => spawnPoints;

    void Awake(){
      City.InteractableLocation[] locations = GetComponentsInChildren<City.InteractableLocation>();
      spawnPoints = new List<SpawnPoint>();
      foreach(City.InteractableLocation location in locations){
        spawnPoints.Add(new SpawnPoint(location.Destination, location.LocationPosition.gameObject));
      }
    }
  }
}