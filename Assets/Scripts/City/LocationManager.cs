using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public interface ILocationManager {
    int GetProgressForLocation(LocationType type);
    int GetProgressForLocationObject(LocationType type, ObjectType obj);
    void IncreaseLocationProgress(LocationType type);
    void IncreaseProgressForLocationObject(LocationType type, ObjectType obj);
  }
  
  public class LocationManager : IInitializable, ILocationManager {
    private Dictionary<LocationType, int> locationProgress;
    private Dictionary<LocationType, Dictionary<ObjectType, int>> locationObjectProgress;
    private LocationType currentLocation;

    public void Initialize() {
      LoadLocationProgress();
      LoadLocationObjectProgress();
    }

    public void IncreaseLocationProgress(LocationType type) {
      locationProgress[type] += 1;
      ResetLocationObjectProgress(type);
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
    
    public int GetProgressForLocation(LocationType type) {
      return locationProgress[type];
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
    
    private void LoadLocationProgress() {
      var locations = Enum.GetValues(typeof(LocationType)).Cast<LocationType>();
      locationProgress = new Dictionary<LocationType, int>();
      foreach (var location in locations) {
        //TODO(dwong): add based on saved state.
        locationProgress.Add(location, 0);
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
