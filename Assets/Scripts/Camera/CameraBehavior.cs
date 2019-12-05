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
    [SerializeField] private bool useBounds;
    [SerializeField] private float smoothSpeed = .125f;
    [SerializeField] private Vector3 offset;

    [Header("Bounds")]
    [SerializeField] private Vector2 minBound;
    [SerializeField] private Vector2 maxBound;

    [Header("X Offset Control")]
    [Tooltip("When player is beyond this distance from the center, move in x direction.")]
    [SerializeField] private float xMoveBound = 1f;
    private bool moveInX;

    [Header("Y Offset Control")]
    [Tooltip("When player is beyond this distance from the center, move in y direction.")]
    [SerializeField] private float yMoveBound = 1f;
    [Tooltip("Distance inside y bounds where camera will just follow the player")]
    [SerializeField] private float yBoundOffset = 3.5f;

    [Header("Look Ahead")]
    [SerializeField] private float xLookAheadOffset = 2;
    [SerializeField] [Range(0, 1)] private float lookAheadSmoothing = .2f;
    private Vector3 currentLookAheadPos;
    private float currentLookAheadSpeed;

    [Inject] private IPlayerMotion player;
    
    private Vector2 currentSpeed = Vector2.zero;
    private Vector3 currentPosition;
    private bool shouldFollow = true;

    private bool movingLeft;

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
      currentLookAheadPos = player.PlayerTransform.position;
    }

    void FixedUpdate() {
      if (!ShouldFollow) {
        currentSpeed = Vector3.zero;
        return;
      }

      if(useBounds){
        MoveBasedOnBounds();
      }
      else{
        MoveTo(player.PlayerTransform.position + offset);
      }
    }

    private void MoveBasedOnBounds(){
      Debug.DrawLine(new Vector3(transform.position.x + xMoveBound, -100),
        new Vector3(transform.position.x + xMoveBound, 100),
        Color.yellow);
      Debug.DrawLine(new Vector3(transform.position.x - xMoveBound, -100),
        new Vector3(transform.position.x - xMoveBound, 100),
        Color.yellow);

      UpdateLookAhead();
      currentLookAheadPos.DrawCrosshair(Color.cyan);

      bool moved = KeepPlayerInView();
      MoveTo(new Vector3(TargetX(), moved ? currentPosition.y : TargetY(), 0),
         Mathf.Abs(player.Velocity.x) * 2f);
    }

    // returns if the player was moved this ways
    private bool KeepPlayerInView(){
      // player exiting bounds, immediately move camera to keep player inside
      Bounds bounds = Camera.main.OrthographicBounds();
      bounds.Expand(new Vector3(0, -yBoundOffset, 100));
      if(!bounds.Contains(player.PlayerTransform.position)){
        Vector3 nearest = bounds.ClosestPoint(player.PlayerTransform.position);
        currentPosition += player.PlayerTransform.position - nearest;

        transform.position = currentPosition;
        return true;
      }

      return false;
    }

    private void UpdateLookAhead(){
      if(Mathf.Abs(player.Velocity.x) < .01f){
        currentLookAheadPos.x = Mathf.SmoothDamp(currentLookAheadPos.x,
          player.PlayerTransform.position.x, 
          ref currentLookAheadSpeed,
          lookAheadSmoothing);
      }
      else if(player.IsFacingLeft){
        currentLookAheadPos.x = Mathf.SmoothDamp(currentLookAheadPos.x,
          player.PlayerTransform.position.x - xLookAheadOffset, 
          ref currentLookAheadSpeed,
          lookAheadSmoothing);
      }
      else{
        currentLookAheadPos.x = Mathf.SmoothDamp(currentLookAheadPos.x,
          player.PlayerTransform.position.x + xLookAheadOffset, 
          ref currentLookAheadSpeed,
          lookAheadSmoothing);
      }

      currentLookAheadPos.y = player.PlayerTransform.position.y;
    }

    private float TargetX(){
      if(moveInX){
        if(movingLeft != player.IsFacingLeft || Mathf.Abs(player.Velocity.x) < .01f){
          moveInX = false;
          return currentPosition.x;
        }
      }
      else{
        float diff = currentLookAheadPos.x - currentPosition.x;
        if(Mathf.Abs(diff) < xMoveBound){
          return currentPosition.x;
        }
        else{
          moveInX = true;
          movingLeft = player.IsFacingLeft;
        }
      }

      if(player.IsFacingLeft){
        return currentLookAheadPos.x;
      }
      return currentLookAheadPos.x;
    }

    private float TargetY(){
      float diff = player.PlayerTransform.position.y - currentPosition.y;
      if(Mathf.Abs(diff) < yMoveBound){
        return currentPosition.y;
      }

      return diff - (Mathf.Sign(diff) * yMoveBound) + currentPosition.y;
    }

    private void MoveTo(Vector3 position, float maxXSpeed = float.PositiveInfinity){
      Vector3 clampedPos = ClampPosition(position);

      clampedPos.DrawCrosshair(Color.magenta);

      currentPosition.x = Mathf.SmoothDamp(transform.position.x, clampedPos.x,
        ref currentSpeed.x, smoothSpeed, maxXSpeed);

      currentPosition.y = Mathf.SmoothDamp(transform.position.y, clampedPos.y,
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