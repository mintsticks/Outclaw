using System;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public class Footprint : MonoBehaviour, ISenseElement {
    public class Factory : PlaceholderFactory<Data, Footprint> { }
    
    public class Data {
      public Transform FootprintSource;
      public float PathDistance;
      public Vector3 Position;
    }

    [SerializeField] private Color minColor;
    [SerializeField] private Color maxColor;
    [SerializeField] private SpriteRenderer sprite;
    
    [Inject] private ISenseVisuals senseVisuals;

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
      senseVisuals.RegisterSenseElement(this);
      gameObject.SetActive(false);
    }

    private void Update() {
      var dist = transform.position - footprintSource.position;
      var relDist = Mathf.Clamp(dist.magnitude / pathDistance, 0, 1);
      currentColor = Color.Lerp(minColor, maxColor, 1 - relDist);
      if (!IsFading) {
        sprite.color = currentColor;
      }
    }

    public void UpdateElement(float animationProgress) {
      var origin = new Color(currentColor.r, currentColor.g, currentColor.b, 0);
      var color = Color.Lerp(origin, currentColor, animationProgress);
      sprite.color = color;
    }
  }
}