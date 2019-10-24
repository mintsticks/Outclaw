using Antlr4.Runtime.Misc;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Outclaw {
  [System.Serializable]
  public class SpawnPoint {
    [SerializeField] private string entryScene;
    [SerializeField] private LocationData entryLocation;
    [SerializeField] private GameObject spawnPoint;

    public string EntryScene => entryScene;
    public LocationData EntryLocation => entryLocation;
    public GameObject Point => spawnPoint;
    public Vector3 PointPosition => spawnPoint.transform.position;
  }
}