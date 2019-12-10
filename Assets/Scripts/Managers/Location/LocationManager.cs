using System;
using System.Collections.Generic;
using System.Linq;
using City;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public interface ILocationManager {
    LocationData CurrentLocation { get; set; }
    void IncreaseProgress(ObjectDialogueData data);
  }

  public class LocationManager : IInitializable, ILocationManager {
    private LocationData currentLocation;

    public LocationData CurrentLocation {
      get => currentLocation;
      set => currentLocation = value;
    }

    private HashSet<ObjectDialogueData> activeData = new HashSet<ObjectDialogueData>();

    [Inject] IGameStateManager gameStateManager;

    public void Initialize() {
      gameStateManager.OnNonpersistReset += Reset;
    }

    public void Reset(){

      foreach(ObjectDialogueData data in activeData){
        data.Reset();
      }
      activeData.Clear();
    }

    public void IncreaseProgress(ObjectDialogueData data){
      data.Increment();
      activeData.Add(data);
    }
  }
}
