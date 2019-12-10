using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Outclaw{
  public class InitLocation : MonoBehaviour
  {
    [SerializeField] LocationData location;
    [Inject] City.ILocationManager locationManager;

    void Start(){
      if(locationManager.CurrentLocation == null){
        locationManager.CurrentLocation = location;
      }
    }
  }
}
