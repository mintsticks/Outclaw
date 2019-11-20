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
    [Tooltip("Amount always to put the camera ahead of the player")]
    [SerializeField] private float defaultXOffset = 1;
    [Tooltip("Multiple of Velocity to add to x offset")]
    [SerializeField] private float velocityInfluence = .1f;

    [Header("Y Offset Control")]
    [Tooltip("Amount always to put relative player")]
    [SerializeField] private float defaultYOffset = 2;
    [Tooltip("Distance inside y bounds where camera will just follow the player")]
    [SerializeField] private float followDist = 1f;

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
      LerpTo(player.PlayerTransform.position + offset);

      // Vector3 dest = new Vector3(
      //   IdealXPos(),
      //   IdealYPos(),
      //   currentPosition.z);
      // smoothedPlayerVelocity = Vector2.Lerp(smoothedPlayerVelocity, player.Velocity, .05f);
      // MoveTo(dest);

      // dest.DrawCrosshair(Color.white);
      // Debug.DrawLine(new Vector2(transform.position.x, 100), new Vector2(transform.position.x, -100), Color.blue);
      // Bounds bounds = Camera.main.OrthographicBounds();
      // bounds.Expand(new Vector3(0, -followDist, 0));
      // Debug.DrawLine(new Vector2(100, bounds.max.y), new Vector2(-100, bounds.max.y), Color.red);
      // Debug.DrawLine(new Vector2(100, bounds.min.y), new Vector2(-100, bounds.min.y), Color.red);
    }

    private float IdealXPos(){
      float x = player.PlayerTransform.position.x;
      float offset = defaultXOffset + (Mathf.Abs(smoothedPlayerVelocity.x) * velocityInfluence);

      if(player.IsFacingLeft){
        x -= offset;
      }
      else {
        x += offset;
      }
      return x;
    }

    private float IdealYPos(){
      if(player.IsGrounded){
        return player.PlayerTransform.position.y + defaultYOffset;
      }

      // player exiting bounds, immediately move camera to keep player inside
      Bounds bounds = Camera.main.OrthographicBounds();
      bounds.Expand(new Vector3(0, -followDist, 100));
      if(!bounds.Contains(player.PlayerTransform.position)){
        Vector3 nearest = bounds.ClosestPoint(player.PlayerTransform.position);
        currentPosition.y += player.PlayerTransform.position.y - nearest.y;
        transform.position = currentPosition;
      }

      return currentPosition.y;
    }

    private void MoveTo(Vector3 position){
      Vector3 clampedPos = ClampPosition(position);

      float xTime = Mathf.Abs(clampedPos.x - currentPosition.x) / maxSpeed.x;
      currentPosition.x = Mathf.SmoothDamp(currentPosition.x, clampedPos.x,
        ref currentSpeed.x, smoothSpeed);

      float yTime = Mathf.Abs(clampedPos.y - currentPosition.y) / maxSpeed.y;
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