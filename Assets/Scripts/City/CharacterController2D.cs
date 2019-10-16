using System;
using System.Linq;
using UnityEngine;

namespace Outclaw {
  public class CharacterController2D : MonoBehaviour {
    [SerializeField]
    [Range(0.001f, 0.3f)]
    private float skinWidth = 0.02f;
    
    [SerializeField]
    private LayerMask platformMask = 0;
    
    [SerializeField]
    private LayerMask triggerMask = 0;
    
    [SerializeField]
    private LayerMask oneWayPlatformMask = 0;
    
    [SerializeField]
    [Range(0f, 90f)]
    private float slopeLimit = 30f;

    [SerializeField]
    private AnimationCurve slopeSpeedMultiplier =
      new AnimationCurve(new Keyframe(-90f, 1.5f), new Keyframe(0f, 1f), new Keyframe(90f, 0f));

    [SerializeField]
    [Range(2, 20)]
    private int totalHorizontalRays = 8;

    [SerializeField]
    [Range(2, 20)]
    private int totalVerticalRays = 4;

    [SerializeField]
    private Transform transform;
    
    [SerializeField]
    private BoxCollider2D boxCollider;

    [SerializeField]
    [Tooltip("Time before hitting ground to play landing")]
    private float landingTime = .1f;

    private CharacterCollisionState2D collisionState;
    private CharacterRaycastOrigins raycastOrigins;
    private float verticalDistanceBetweenRays;
    private float horizontalDistanceBetweenRays;
    
    private bool isGoingUpSlope;
    private bool isJumping;
    private bool isDescending;
    private Collider2D descendingColliderToIgnore;
    
    private Vector3 deltaMovement;
    private Vector3 velocity;
    
    private const float skinInset = 0.02f;
    private readonly float slopeLimitTangent = Mathf.Tan(75f * Mathf.Deg2Rad);
    
    public Vector3 Velocity => velocity;
    public bool isGrounded => collisionState.below;

    public bool IsNearGround(float gravity){
      float dist = (landingTime * Mathf.Abs(velocity.y)) + (.5f * gravity * landingTime * landingTime);

      Vector3 origin = raycastOrigins.bottomLeft;
      for (var i = 0; i < totalVerticalRays; i++) {
        Vector3 ray = new Vector2(origin.x + i * horizontalDistanceBetweenRays, origin.y);
        RaycastHit2D hit = Physics2D.Raycast(ray,
          Vector3.down, dist, platformMask | oneWayPlatformMask);

        if(hit.collider != null)
          return true;
      }
      return false;
    }
    
    private void Awake() {
      SetUpMasks();
      RecalculateDistanceBetweenRays();
      collisionState = new CharacterCollisionState2D();
    }

    private void SetUpMasks() {
      platformMask |= oneWayPlatformMask;
      for (var i = 0; i < 32; i++) {
        if ((triggerMask.value & 1 << i) == 0)
          Physics2D.IgnoreLayerCollision(gameObject.layer, i);
      }
    }

    private void ResetStates() {
      collisionState.wasGroundedLastFrame = collisionState.below;
      collisionState.below = false;
      isGoingUpSlope = false;
    }
    
    public void Move(Vector3 _deltaMovement, ref bool _isJumping, ref bool _isDescending) {
      deltaMovement = _deltaMovement;
      isJumping = _isJumping;
      isDescending = _isDescending;

      ResetStates();
      UpdateRaycastOrigins();

      if (deltaMovement.y < 0 && collisionState.wasGroundedLastFrame) {
        CheckVerticalSlope();
      }
      
      UpdateHorizontal();
      UpdateVertical();

      if (Math.Abs(deltaMovement.y) < .00001f) {
        collisionState.below = true;
      }
      
      if (isDescending) {
        deltaMovement.y = Math.Max(deltaMovement.y, -.1f);
      }
      deltaMovement.z = 0;
      transform.Translate(deltaMovement, Space.World);
      _isJumping = false;
      _isDescending = false;
      
      velocity = deltaMovement / Time.fixedDeltaTime;
      if (isGoingUpSlope) {
        velocity.y = 0;
      }
    }

    private void UpdateHorizontal() {
      var isGoingRight = deltaMovement.x > 0;
      var rayDistance = Mathf.Abs(deltaMovement.x) + skinWidth;
      var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
      var initialRayOrigin = isGoingRight ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;

      var onSlope = CheckHorizontalSlope(rayDirection, initialRayOrigin, rayDistance);
      if (onSlope) {
        return;
      }
      
      CheckHorizontalRays(rayDirection, initialRayOrigin,rayDistance, isGoingRight);
    }
    
    private bool CheckHorizontalSlope(Vector2 rayDirection, Vector3 origin, float rayDistance) {
      var collisionMask = collisionState.wasGroundedLastFrame ? (int)platformMask : platformMask & ~oneWayPlatformMask;
      var raycastHit = Physics2D.Raycast(origin, rayDirection, rayDistance, collisionMask);
      if (!raycastHit) {
        return false;
      }

      var slopeAngle = Vector2.Angle(raycastHit.normal, Vector2.up);
      var hasValidSlope = HandleHorizontalSlope(slopeAngle);
      if (!hasValidSlope) {
        return false;
      }
      
      if (collisionState.wasGroundedLastFrame) {
        return true;
      }
      
      var flushDistance = Mathf.Sign(deltaMovement.x) * (raycastHit.distance - skinWidth);
      transform.Translate(new Vector2(flushDistance, 0));
      return true;
    }

    private void CheckHorizontalRays(Vector2 rayDirection, Vector3 initialRayOrigin, float rayDistance, bool isGoingRight) {
      for (var i = 0; i < totalHorizontalRays; i++) {
        var ray = new Vector2(initialRayOrigin.x, initialRayOrigin.y + i * verticalDistanceBetweenRays);
        var raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, platformMask & ~oneWayPlatformMask);
        if (!raycastHit) {
          continue;
        }

        deltaMovement.x = raycastHit.point.x - ray.x;
        rayDistance = Mathf.Abs(deltaMovement.x);
        
        if (isGoingRight) {
          deltaMovement.x -= skinWidth;
        } else {
          deltaMovement.x += skinWidth;
        }
      }
    }

    private bool HandleHorizontalSlope(float angle) {
      if(Mathf.RoundToInt(angle) == 90 || isDescending) {
        return false;
      }
      
      if (angle > slopeLimit) {
        deltaMovement.x = 0;
        return true;
      }

      if (isJumping) {
        return true;
      }
      
      
      deltaMovement.x *= slopeSpeedMultiplier.Evaluate(angle); 
      deltaMovement.y = Mathf.Abs(Mathf.Tan(angle * Mathf.Deg2Rad) * deltaMovement.x);

      CheckModifiedHorizontalLocation();
      isGoingUpSlope = true;
      collisionState.below = true;
      return true;
    }

    private void CheckModifiedHorizontalLocation() {
      var isGoingRight = deltaMovement.x > 0;
      var ray = isGoingRight ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
      var raycastHit = Physics2D.Raycast(ray, deltaMovement.normalized, deltaMovement.magnitude,
        platformMask & ~oneWayPlatformMask);

      if (!raycastHit) {
        return;
      }
      
      deltaMovement = (Vector3) raycastHit.point - ray;
      deltaMovement.x += isGoingRight ? -skinWidth : skinWidth;
    }

    private void UpdateVertical() {
      var isGoingUp = deltaMovement.y > 0.00001;
      var rayDistance = Mathf.Abs(deltaMovement.y) + skinWidth;
      var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
      var initialRayOrigin = isGoingUp ? raycastOrigins.topLeft : raycastOrigins.bottomLeft;
      initialRayOrigin.x += deltaMovement.x;
      
      CheckVerticalRays(rayDirection, initialRayOrigin, rayDistance, isGoingUp);
    }

    private void CheckDescend(Vector2 ray, Vector2 rayDirection, float rayDistance) {
      if (!isDescending) {
        return;
      }
      var oneWayPlatformRaycast = Physics2D.Raycast(ray, rayDirection, rayDistance, oneWayPlatformMask);
      if (oneWayPlatformRaycast) {
        descendingColliderToIgnore = oneWayPlatformRaycast.collider;
      }
    }

    private void CheckVerticalRays(Vector2 rayDirection, Vector3 initialRayOrigin, float rayDistance, bool isGoingUp) {
      var modifiedTopLeft =
        raycastOrigins.topLeft + new Vector3(skinInset, -skinInset, 0);
      var modifiedBottomRight =
        raycastOrigins.bottomRight + new Vector3(-skinInset, skinInset, 0);
      
      var overlaps = Physics2D.OverlapAreaAll(modifiedTopLeft, modifiedBottomRight, oneWayPlatformMask);
      var mask = isGoingUp ? platformMask & ~oneWayPlatformMask : (int)platformMask;
      for (var i = 0; i < totalVerticalRays; i++) {
        var ray = new Vector2(initialRayOrigin.x + i * horizontalDistanceBetweenRays, initialRayOrigin.y);
        Debug.DrawRay(ray, rayDirection, Color.blue);
        var raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, mask);
        if (!raycastHit) {
          continue;
        }

        CheckDescend(ray, rayDirection, rayDistance);
        if (!isGoingUp && 
            (overlaps.Contains(raycastHit.collider) || 
             raycastHit.collider == descendingColliderToIgnore)) {
          continue;
        }
        
        deltaMovement.y = raycastHit.point.y - ray.y;
        rayDistance = Mathf.Abs(deltaMovement.y);
        if (isGoingUp) {
          deltaMovement.y -= skinWidth;
        } else {
          deltaMovement.y += skinWidth;
          collisionState.below = true;
          descendingColliderToIgnore = null;
        }

        if (!isGoingUp && deltaMovement.y > 0.00001f) {
          isGoingUpSlope = true;
        }
      }
    }
    
    private void CheckVerticalSlope() {
      var bottomCenter = (raycastOrigins.bottomLeft.x + raycastOrigins.bottomRight.x) * 0.5f;
      var origin = new Vector2(bottomCenter, raycastOrigins.bottomLeft.y);
      var rayDistance = slopeLimitTangent * (raycastOrigins.bottomRight.x - bottomCenter);
      var rayDirection = -Vector2.up;
      var raycastHit = Physics2D.Raycast(origin, rayDirection, rayDistance, platformMask);
      if (!raycastHit) {
        return;
      }

      var angle = Vector2.Angle(raycastHit.normal, Vector2.up);
      if (Mathf.RoundToInt(angle) == 0) {
        return;
      }
      var isMovingDownSlope = raycastHit.normal.x * deltaMovement.x > 0;
      if (!isMovingDownSlope) {
        return;
      }

      if (isDescending) {
        deltaMovement.y *= 2f;
        return;
      }
      
      var updatedSpeed = deltaMovement.x * slopeSpeedMultiplier.Evaluate(-angle);
      deltaMovement.y += raycastHit.point.y - origin.y - skinWidth;
      deltaMovement = new Vector3(0, deltaMovement.y, 0) + 
                      Quaternion.AngleAxis(-angle, Vector3.forward) * 
                      new Vector3(updatedSpeed, 0, 0);
    }

    private void RecalculateDistanceBetweenRays() {
      var scale = transform.localScale;
      var skinOffset = 2f * skinWidth;
      verticalDistanceBetweenRays = (boxCollider.size.y * Mathf.Abs(scale.y) - skinOffset) / (totalHorizontalRays - 1);
      horizontalDistanceBetweenRays = (boxCollider.size.x * Mathf.Abs(scale.x) - skinOffset) / (totalVerticalRays - 1);
    }
    
    private void UpdateRaycastOrigins() {
      var modifiedBounds = boxCollider.bounds;
      modifiedBounds.Expand(-2f * skinWidth);
      raycastOrigins.topLeft = new Vector2(modifiedBounds.min.x, modifiedBounds.max.y);
      raycastOrigins.bottomRight = new Vector2(modifiedBounds.max.x, modifiedBounds.min.y);
      raycastOrigins.bottomLeft = modifiedBounds.min;
    }
    
    struct CharacterRaycastOrigins {
      public Vector3 topLeft;
      public Vector3 bottomRight;
      public Vector3 bottomLeft;
    }

    class CharacterCollisionState2D {
      public bool below;
      public bool wasGroundedLastFrame;
    }
  }
}