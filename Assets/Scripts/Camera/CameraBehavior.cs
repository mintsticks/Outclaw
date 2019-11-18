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
    
    void Start(){
      SnapToPlayer();
    }

    void FixedUpdate() {
      if (!ShouldFollow) {
        return;
      }
      var desiredPos = player.PlayerTransform.position + offset;
      var smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
      ClampPosition(ref smoothedPos);
      currentPosition = smoothedPos;

      if (!ShouldFollow) {
        return;
      }
      transform.position = currentPosition;
    }

    private void ClampPosition(ref Vector3 pos){
      pos.x = Mathf.Clamp(pos.x, minBound.x, maxBound.x);
      pos.y = Mathf.Clamp(pos.y, minBound.y, maxBound.y);
    }

    private void SnapToPlayer(){
      Vector3 newPos = player.PlayerTransform.position;
      ClampPosition(ref newPos);
      transform.position = newPos;
    }
  }
}