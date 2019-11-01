using System;
using System.Collections.Generic;
using System.Linq;
using City;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public interface ILocationManager {
    void IncreaseProgress(ObjectDialogueData data);
  }

  public class LocationManager : IInitializable, ILocationManager, IResetableManager  {
    private LocationData currentLocation;

    private HashSet<ObjectDialogueData> activeData = new HashSet<ObjectDialogueData>();

    [Inject] IGameStateManager gameStateManager;

    public void Initialize() {
      gameStateManager.RegisterNonpersistResetOnStateChange(this);
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
