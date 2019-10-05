using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public interface ILocationManager {
    int GetProgressForLocationObject(LocationType type, ObjectType obj);
    void IncreaseProgressForLocationObject(LocationType type, ObjectType obj);
    void ResetObjectProgress();
  }
  
  public class LocationManager : IInitializable, ILocationManager {
    private Dictionary<LocationType, Dictionary<ObjectType, int>> locationObjectProgress;
    private LocationType currentLocation;

    public void Initialize() {
      LoadLocationObjectProgress();
    }

    public void IncreaseProgressForLocationObject(LocationType type, ObjectType obj) {
      if (!locationObjectProgress.TryGetValue(type, out var locationObjects)) {
        Debug.LogError("Location " + type + " does not exist.");
        return;
      }

      if (!locationObjects.ContainsKey(obj)) {
        locationObjects[obj] = 0;
      }

      locationObjects[obj]++;
    }

    public int GetProgressForLocationObject(LocationType type, ObjectType obj) {
      if (!locationObjectProgress.TryGetValue(type, out var locationObjects)) {
        Debug.LogError("Location " + type + " does not exist.");
        return 0;
      }

      if (!locationObjects.ContainsKey(obj)) {
        locationObjects[obj] = 0;
      }

      return locationObjects[obj];
    }

    // Call this when updating game state.
    // TODO:(dwong) if we allow for cycles in our game states graph, we may have to store per game state progress.
    public void ResetObjectProgress() {
      foreach (var loc in locationObjectProgress.Keys) {
        ResetLocationObjectProgress(loc);
      }
    }
    
    private void ResetLocationObjectProgress(LocationType type) {
      var locationObjects = locationObjectProgress[type];
      foreach (var obj in locationObjects.Keys) {
        locationObjects[obj] = 0;
      }
    }
    
    private void LoadLocationObjectProgress() {
      var locations = Enum.GetValues(typeof(LocationType)).Cast<LocationType>();
      locationObjectProgress = new Dictionary<LocationType, Dictionary<ObjectType, int>>();
      foreach (var location in locations) {
        //TODO(dwong): add based on saved state.
        locationObjectProgress.Add(location, new Dictionary<ObjectType, int>());
      }
    }
  }
  
  public enum LocationType {
    NONE,
    OUTSIDE,
    HOUSE,
    CAFE
  }
}
