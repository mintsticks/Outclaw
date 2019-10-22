using Outclaw.Heist;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public interface ICameraBehavior {
    bool ShouldFollow { get; set; }
  }
  
  public class CameraBehavior : MonoBehaviour, ICameraBehavior {
    [SerializeField] private float smoothSpeed = .125f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector2 minBound;
    [SerializeField] private Vector2 maxBound;

    [Inject] private IPlayer player;

    public bool ShouldFollow { get; set; }

    public Vector2 MinBound => minBound;
    public Vector2 MaxBound => maxBound;

    void FixedUpdate() {
      if (ShouldFollow) {
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