using Antlr4.Runtime.Misc;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Outclaw {
  [System.Serializable]
  public class SpawnPoint {
    [SerializeField] private LocationData entryLocation;
    [SerializeField] private GameObject spawnPoint;

    public LocationData EntryLocation => entryLocation;
    public GameObject Point => spawnPoint;
    public Vector3 PointPosition => spawnPoint.transform.position;

    public SpawnPoint(LocationData location, GameObject spawn){
      entryLocation = location;
      spawnPoint = spawn;
    }
  }
}