using System;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw {
  public class SpawnList : MonoBehaviour {
    [SerializeField] private List<SpawnPoint> spawnPoints;

    public List<SpawnPoint> SpawnPoints => spawnPoints;
  }
}