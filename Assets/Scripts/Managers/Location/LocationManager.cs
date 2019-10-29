using System;
using System.Collections.Generic;
using System.Linq;
using City;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public interface ILocationManager {
    int GetProgressForLocationObject(LocationData location, ObjectType obj);
    void IncreaseProgressForLocationObject(LocationData location, ObjectType obj);
    void ResetObjectProgress();
  }

  public class LocationManager : IInitializable, ILocationManager {
    private Dictionary<LocationData, Dictionary<ObjectType, int>> locationObjectProgress;
    private LocationData currentLocation;

    public void Initialize() {
      LoadLocationObjectProgress();
    }

    public void IncreaseProgressForLocationObject(LocationData location, ObjectType obj) {
      if (!locationObjectProgress.TryGetValue(location, out var locationObjects))
      {
        Debug.LogError("Location " + location + " does not exist.");
        return;
      }

      if (!locationObjects.ContainsKey(obj))
      {
        locationObjects[obj] = 0;
      }

      locationObjects[obj]++;
    }

    public int GetProgressForLocationObject(LocationData location, ObjectType obj) {
      if (!locationObjectProgress.TryGetValue(location, out var locationObjects))
      {
        Debug.LogError("Location " + location + " does not exist.");
        return 0;
      }

      if (!locationObjects.ContainsKey(obj))
      {
        locationObjects[obj] = 0;
      }

      return locationObjects[obj];
    }

    // Call this when updating game state.
    // TODO:(dwong) if we allow for cycles in our game states graph, we may have to store per game state progress.
    public void ResetObjectProgress() {
      foreach (var loc in locationObjectProgress.Keys)
      {
        ResetLocationObjectProgress(loc);
      }
    }

    private void ResetLocationObjectProgress(LocationData location) {
      var locationObjects = locationObjectProgress[location];


      // make a copy of keys so foreach can still go while modifying
      var keys = new List<ObjectType>(locationObjects.Keys);
      foreach (var obj in keys) {
        locationObjects[obj] = 0;
      }
    }

    private void LoadLocationObjectProgress() {
      var locations = Resources.LoadAll<LocationData>("Location Data");
      locationObjectProgress = new Dictionary<LocationData, Dictionary<ObjectType, int>>();
      foreach (var location in locations)
      {
        //TODO(dwong): add based on saved state.
        locationObjectProgress.Add(location, new Dictionary<ObjectType, int>());
      }
    }
  }
}
