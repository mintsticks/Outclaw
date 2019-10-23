using Outclaw.Heist;
using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public interface ICameraBehavior {
    bool ShouldFollow { get; set; }
    Vector3 Offset { get; set; }
    Vector3 GetCurrentCameraPos();
  }
  
  public class CameraBehavior : MonoBehaviour, ICameraBehavior {
    [SerializeField] private float smoothSpeed = .125f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector2 minBound;
    [SerializeField] private Vector2 maxBound;

    [Inject] private IPlayer player;
    
    private Vector3 currentPosition;
    private bool shouldFollow = true;

    public bool ShouldFollow {
      get => shouldFollow;
      set => shouldFollow = value;
    }

    public Vector3 Offset { 
      get => offset;
      set => offset = value;
    }

    public Vector2 MinBound => minBound;
    public Vector2 MaxBound => maxBound;

    public Vector3 GetCurrentCameraPos() {
      return currentPosition;
    }
    
    void FixedUpdate() {
      if (!ShouldFollow) {
        return;
      }
      var desiredPos = player.PlayerTransform.position + offset;
      var smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
      smoothedPos.x = Mathf.Clamp(smoothedPos.x, minBound.x, maxBound.x);
      smoothedPos.y = Mathf.Clamp(smoothedPos.y, minBound.y, maxBound.y);
      currentPosition = smoothedPos;

      if (!ShouldFollow) {
        return;
      }
      transform.position = currentPosition;
    }
  }
}