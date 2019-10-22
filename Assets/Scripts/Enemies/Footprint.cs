using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public class Footprint : MonoBehaviour {
    public class Factory : PlaceholderFactory<Data, Footprint> { }
    
    public class Data {
      public Transform FootprintSource;
      public float PathDistance;
      public Vector3 Position;
    }

    [SerializeField] private Color minColor;
    [SerializeField] private Color maxColor;
    [SerializeField] private SpriteRenderer sprite;
    
    private Transform footprintSource;
    private float pathDistance;
    
    [Inject]
    public void Initialize(Data data) {
      footprintSource = data.FootprintSource;
      pathDistance = data.PathDistance;
      transform.position = data.Position;
    }
    
    private void Update() {
      var dist = transform.position - footprintSource.position;
      Debug.Log(dist.magnitude);
      var relDist = Mathf.Clamp(dist.magnitude / pathDistance, 0, 1);
      sprite.color = Color.Lerp(minColor, maxColor, 1 - relDist);
    }
  }
}