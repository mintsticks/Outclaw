using Outclaw.Heist;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public class CameraBehavior : MonoBehaviour {
    [SerializeField] private float smoothSpeed = .125f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector2 minBound;
    [SerializeField] private Vector2 maxBound;

    [Inject] private IPlayer player;
    [Inject] private IVantagePointManager vantagePointManager;

    public Vector2 MinBound => minBound;
    public Vector2 MaxBound => maxBound;

    void FixedUpdate() {
      if (vantagePointManager.InVantage) {
        return;
      }
      var desiredPos = player.PlayerTransform.position + offset;
      var smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
      smoothedPos.x = Mathf.Clamp(smoothedPos.x, minBound.x, maxBound.x);
      smoothedPos.y = Mathf.Clamp(smoothedPos.y, minBound.y, maxBound.y);
      transform.position = smoothedPos;
    }
  }
}