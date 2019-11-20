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

    [Header("Bounds")]
    [SerializeField] private Vector2 minBound;
    [SerializeField] private Vector2 maxBound;

    [Header("Movement")]
    [SerializeField] private Vector2 maxSpeed = new Vector2(1f, 1f);

    [Header("X Offset Control")]
    [SerializeField] private float defaultXOffset = 1;
    [Tooltip("Multiple of Velocity to add to x offset")]
    [SerializeField] private float velocityInfluence = .1f;

    [Header("Y Offset Control")]
    [Tooltip("Distance inside y bounds where camera will just follow the player")]
    [SerializeField] private float yBoundOffset = 3.5f;

    [Inject] private IPlayerMotion player;
    
    private Vector2 currentSpeed = Vector2.zero;
    private Vector3 currentPosition;
    private bool shouldFollow = true;
    private Vector2 smoothedPlayerVelocity;

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
      currentPosition = transform.position;
      SnapToPlayer();
    }

    void FixedUpdate() {
      if (!ShouldFollow) {
        return;
      }

      KeepPlayerInView();
      MoveTo(player.PlayerTransform.position + offset);
    }

    private void KeepPlayerInView(){
      // player exiting bounds, immediately move camera to keep player inside
      Bounds bounds = Camera.main.OrthographicBounds();
      bounds.Expand(new Vector3(0, -yBoundOffset, 100));
      if(!bounds.Contains(player.PlayerTransform.position)){
        Vector3 nearest = bounds.ClosestPoint(player.PlayerTransform.position);
        currentPosition += player.PlayerTransform.position - nearest;

        currentPosition = ClampPosition(currentPosition);
        transform.position = currentPosition;
      }
    }

    private void MoveTo(Vector3 position){
      Vector3 clampedPos = ClampPosition(position);

      currentPosition.x = Mathf.SmoothDamp(currentPosition.x, clampedPos.x,
        ref currentSpeed.x, smoothSpeed);

      currentPosition.y = Mathf.SmoothDamp(currentPosition.y, clampedPos.y,
        ref currentSpeed.y, smoothSpeed);
      transform.position = currentPosition;
    }

    private void LerpTo(Vector3 position){
      var smoothedPos = Vector3.Lerp(transform.position, position, smoothSpeed);
      currentPosition = ClampPosition(smoothedPos);
      transform.position = currentPosition;
    }

    private Vector3 ClampPosition(Vector3 pos){
      return new Vector3(
        Mathf.Clamp(pos.x, minBound.x, maxBound.x),
        Mathf.Clamp(pos.y, minBound.y, maxBound.y),
        currentPosition.z);
    }

    private void SnapToPlayer(){
      Vector3 newPos = player.PlayerTransform.position;
      currentPosition = ClampPosition(newPos);
      transform.position = currentPosition;
    }
  }
}