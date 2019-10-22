using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public class FootprintManager : MonoBehaviour {
    [SerializeField] private Transform guardTransform;
    [SerializeField] private LineRenderer path;
    [SerializeField] private float footprintsPerSegment;
    [Inject] private Footprint.Factory footprintFactory;
    
    private void Awake() {
      if (path.positionCount <= 1) {
        return;
      }

      var start = path.GetPosition(0);
      var end = path.GetPosition(path.positionCount - 1);
      var pathDistance = (start - end).magnitude;
      for (var i = 0; i < path.positionCount - 1; i++) {
        CreateFootprintsBetweenPoints(path.GetPosition(i), path.GetPosition(i+1), pathDistance);
      }
      CreateFootprintAtPoint(end, pathDistance);
    }

    private void CreateFootprintsBetweenPoints(Vector2 start, Vector2 end, float pathDistance) {
      for (var k = 0; k < footprintsPerSegment; k++) {
        var pos = Vector2.Lerp(start, end, k / footprintsPerSegment);
        var footprint = footprintFactory.Create(new Footprint.Data {
          Position = pos,
          FootprintSource = guardTransform,
          PathDistance = pathDistance
        });
        footprint.transform.parent = transform;
      }
    }

    private void CreateFootprintAtPoint(Vector2 point, float pathDistance) {
      var footprint = footprintFactory.Create(new Footprint.Data {
        Position = point,
        FootprintSource = guardTransform,
        PathDistance = pathDistance
      });
      footprint.transform.parent = transform;
    }
  }
}