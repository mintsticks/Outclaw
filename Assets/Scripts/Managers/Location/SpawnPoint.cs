using Antlr4.Runtime.Misc;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Outclaw {
  public class SpawnPoint : Pair<string, GameObject> {
    public SpawnPoint(string entryScene, GameObject spawnPoint) : base(entryScene, spawnPoint) {}

    public string GetScene() {
      return a;
    }

    public Vector3 GetSpawnPosition() {
      return b.transform.position;
    }
  }
}