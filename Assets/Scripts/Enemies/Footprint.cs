using System;
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

    [Inject] private IHeistSenseManager senseManager;
    
    public SpriteRenderer sprite;
    public bool IsFading;
    
    private Color currentColor;
    private Transform footprintSource;
    private float pathDistance;
    
    [Inject]
    public void Initialize(Data data) {
      footprintSource = data.FootprintSource;
      pathDistance = data.PathDistance;
      transform.position = data.Position;
    }

    public void Awake() {
      senseManager.RegisterFootprint(this);
      gameObject.SetActive(false);
    }

    public Color CurrentColor() {
      return currentColor;
    }
    
    private void Update() {
      var dist = transform.position - footprintSource.position;
      var relDist = Mathf.Clamp(dist.magnitude / pathDistance, 0, 1);
      currentColor = Color.Lerp(minColor, maxColor, 1 - relDist);
      if (!IsFading) {
        sprite.color = currentColor;
      }
    }
  }
}