using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw{
  [CreateAssetMenu(fileName = "Location Data", menuName = "Outclaw/Location Data")]
  public class LocationData : ScriptableObject
  {
    [SerializeField] private string sceneName;
    [SerializeField] private List<LocationData> exits;

    public string SceneName { get => sceneName; }

    // returns the next location to get to destination
    //   if finding path to self, returns self
    //   if no path exists, returns null
    public LocationData NextLocationTo(LocationData destination){
      // already have a connection, return it
      if(this == destination || exits.Contains(destination)){
        return destination;
      }

      // BFS to search
      Queue<LocationData> toVisit = new Queue<LocationData>();
      HashSet<LocationData> visited = new HashSet<LocationData>();
      Dictionary<LocationData, LocationData> sourceMap = new Dictionary<LocationData, LocationData>();
      toVisit.Enqueue(this);

      while(toVisit.Count != 0){
        LocationData curr = toVisit.Dequeue();
        if(visited.Contains(curr)){
          continue;
        }
        visited.Add(curr);

        foreach(LocationData loc in curr.exits){
          // found, return source that connects to this
          if(loc == destination){
            return GetConnectedSource(sourceMap, curr);
          }

          // already in the queue
          if(sourceMap.ContainsKey(loc)){
            continue;
          }

          // visit it later
          toVisit.Enqueue(loc);
          sourceMap.Add(loc, curr);
        }
      }

      return null;
    }

    private LocationData GetConnectedSource(Dictionary<LocationData, LocationData> sourceMap,
        LocationData start){
      while(sourceMap[start] != this){
        start = sourceMap[start];
      }

      return start;
    }
  }
}
