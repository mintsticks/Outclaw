using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public class FootprintManager : MonoBehaviour {
    [SerializeField] private Transform guardTransform;
    [SerializeField] private LineRenderer path;

    [Inject] private Footprint.Factory footprintFactory;
    
    private List<Footprint> footprints;

    private void Awake() {
      if (path.positionCount <= 1) {
        return;
      }

      var start = path.GetPosition(0);
      var end = path.GetPosition(path.positionCount - 1);
      var pathDistance = (start - end).magnitude;
      for (var i = 0; i < path.positionCount; i++) {
        footprintFactory.Create(new Footprint.Data {
          Position = path.GetPosition(i),
          FootprintSource = guardTransform,
          PathDistance = pathDistance
        });
      }
      
    }
  }
}