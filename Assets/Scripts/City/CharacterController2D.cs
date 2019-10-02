#define DEBUG_CC2D_RAYS
using System;
using UnityEngine;

namespace Outclaw {
  public class CharacterController2D : MonoBehaviour {
    struct CharacterRaycastOrigins {
      public Vector3 topLeft;
      public Vector3 bottomRight;
      public Vector3 bottomLeft;
    }

    class CharacterCollisionState2D {
      public bool below;
      public bool wasGroundedLastFrame;
    }
    
    [System.Diagnostics.Conditional("DEBUG_CC2D_RAYS")]
    static void DrawRay(Vector3 start, Vector3 dir, Color color) {
      Debug.DrawRay(start, dir, color);
    }

    const float kSkinWidthFloatFudgeFactor = 0.001f;
    private readonly float slopeLimitTangent = Mathf.Tan(75f * Mathf.Deg2Rad);
    
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

    /// <summary>
    /// the threshold in the change in vertical movement between frames that constitutes jumping
    /// </summary>
    /// <value>The jumping threshold.</value>
    public float jumpingThreshold = 0.07f;

    
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
    private new Transform transform;
    
    [SerializeField]
    private BoxCollider2D boxCollider;

    private CharacterCollisionState2D collisionState;
    private Vector3 velocity;
    private CharacterRaycastOrigins _raycastOrigins;
    private RaycastHit2D _raycastHit;
    private float verticalDistanceBetweenRays;
    private float horizontalDistanceBetweenRays;
    private bool isGoingUpSlope;
    private bool isJumping;
    
    public Vector3 Velocity => velocity;
    public bool isGrounded => collisionState.below;


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


    /// <summary>
    /// attempts to move the character to position + deltaMovement. Any colliders in the way will cause the movement to
    /// stop when run into.
    /// </summary>
    /// <param name="deltaMovement">Delta movement.</param>
    public void move(Vector3 deltaMovement, ref bool isJumping) {
      collisionState.wasGroundedLastFrame = collisionState.below;
      collisionState.below = false;
      isGoingUpSlope = false;
      this.isJumping = isJumping;
      UpdateRaycastOrigins();


      // first, we check for a slope below us before moving
      // only check slopes if we are going down and grounded
      if (deltaMovement.y < kSkinWidthFloatFudgeFactor && collisionState.wasGroundedLastFrame)
        handleVerticalSlope(ref deltaMovement);

      // now we check movement in the horizontal dir
      if (Math.Abs(deltaMovement.x) > kSkinWidthFloatFudgeFactor)
        moveHorizontally(ref deltaMovement);

      // next, check movement in the vertical dir
      if (Math.Abs(deltaMovement.y) > kSkinWidthFloatFudgeFactor)
        moveVertically(ref deltaMovement);
      
      // move then update our state
      deltaMovement.z = 0;
      transform.Translate(deltaMovement, Space.World);
      isJumping = false;
      // only calculate velocity if we have a non-zero deltaTime
      if (Time.deltaTime > 0f)
        velocity = deltaMovement / Time.deltaTime;

      // if we are going up a slope we artificially set a y velocity so we need to zero it out here
      if (isGoingUpSlope) {
        velocity.y = 0;
      }
    }

    public void RecalculateDistanceBetweenRays() {
      var scale = transform.localScale;
      var colliderUseableHeight = boxCollider.size.y * Mathf.Abs(scale.y) - (2f * skinWidth);
      var colliderUseableWidth = boxCollider.size.x * Mathf.Abs(scale.x) - (2f * skinWidth);
      
      verticalDistanceBetweenRays = colliderUseableHeight / (totalHorizontalRays - 1);
      horizontalDistanceBetweenRays = colliderUseableWidth / (totalVerticalRays - 1);
    }

    /// <summary>
    /// resets the raycastOrigins to the current extents of the box collider inset by the skinWidth. It is inset
    /// to avoid casting a ray from a position directly touching another collider which results in wonky normal data.
    /// </summary>
    /// <param name="futurePosition">Future position.</param>
    /// <param name="deltaMovement">Delta movement.</param>
    void UpdateRaycastOrigins() {
      // our raycasts need to be fired from the bounds inset by the skinWidth
      var modifiedBounds = boxCollider.bounds;
      modifiedBounds.Expand(-2f * skinWidth);

      _raycastOrigins.topLeft = new Vector2(modifiedBounds.min.x, modifiedBounds.max.y);
      _raycastOrigins.bottomRight = new Vector2(modifiedBounds.max.x, modifiedBounds.min.y);
      _raycastOrigins.bottomLeft = modifiedBounds.min;
    }


    /// <summary>
    /// we have to use a bit of trickery in this one. The rays must be cast from a small distance inside of our
    /// collider (skinWidth) to avoid zero distance rays which will get the wrong normal. Because of this small offset
    /// we have to increase the ray distance skinWidth then remember to remove skinWidth from deltaMovement before
    /// actually moving the player
    /// </summary>
    void moveHorizontally(ref Vector3 deltaMovement) {
      var isGoingRight = deltaMovement.x > 0;
      var rayDistance = Mathf.Abs(deltaMovement.x) + skinWidth;
      var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
      var initialRayOrigin = isGoingRight ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;

      for (var i = 0; i < totalHorizontalRays; i++) {
        var ray = new Vector2(initialRayOrigin.x, initialRayOrigin.y + i * verticalDistanceBetweenRays);

        DrawRay(ray, rayDirection * rayDistance, Color.red);

        // if we are grounded we will include oneWayPlatforms only on the first ray (the bottom one). this will allow us to
        // walk up sloped oneWayPlatforms
        if (i == 0 && collisionState.wasGroundedLastFrame)
          _raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, platformMask);
        else
          _raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, platformMask & ~oneWayPlatformMask);

        if (_raycastHit) {
          // the bottom ray can hit a slope but no other ray can so we have special handling for these cases
          if (i == 0 && handleHorizontalSlope(ref deltaMovement, Vector2.Angle(_raycastHit.normal, Vector2.up))) {
            // if we weren't grounded last frame, that means we're landing on a slope horizontally.
            // this ensures that we stay flush to that slope
            if (!collisionState.wasGroundedLastFrame) {
              float flushDistance = Mathf.Sign(deltaMovement.x) * (_raycastHit.distance - skinWidth);
              transform.Translate(new Vector2(flushDistance, 0));
            }

            break;
          }

          // set our new deltaMovement and recalculate the rayDistance taking it into account (readjust for non slope collision)
          deltaMovement.x = _raycastHit.point.x - ray.x;
          rayDistance = Mathf.Abs(deltaMovement.x);

          // remember to remove the skinWidth from our deltaMovement
          if (isGoingRight) {
            deltaMovement.x -= skinWidth;
          } else {
            deltaMovement.x += skinWidth;
          }

          // we add a small fudge factor for the float operations here. if our rayDistance is smaller
          // than the width + fudge bail out because we have a direct impact
          if (rayDistance < skinWidth + kSkinWidthFloatFudgeFactor)
            break;
        }
      }
    }


    /// <summary>
    /// handles adjusting deltaMovement if we are going up a slope.
    /// </summary>
    /// <returns><c>true</c>, if horizontal slope was handled, <c>false</c> otherwise.</returns>
    /// <param name="deltaMovement">Delta movement.</param>
    /// <param name="angle">Angle.</param>
    bool handleHorizontalSlope(ref Vector3 deltaMovement, float angle) {
      // disregard 90 degree angles (walls)
      if (Mathf.RoundToInt(angle) == 90)
        return false;

      // if we can walk on slopes and our angle is small enough we need to move up
      if (angle < slopeLimit) {
        // we only need to adjust the deltaMovement if we are not jumping
        // TODO: this uses a magic number which isn't ideal! The alternative is to have the user pass in if there is a jump this frame
        if (!isJumping) {
          // apply the slopeModifier to slow our movement up the slope
          var slopeModifier = slopeSpeedMultiplier.Evaluate(angle);
          deltaMovement.x *= slopeModifier;

          // we dont set collisions on the sides for this since a slope is not technically a side collision.
          // smooth y movement when we climb. we make the y movement equivalent to the actual y location that corresponds
          // to our new x location using our good friend Pythagoras
          deltaMovement.y = Mathf.Abs(Mathf.Tan(angle * Mathf.Deg2Rad) * deltaMovement.x);
          var isGoingRight = deltaMovement.x > 0;

          // safety check. we fire a ray in the direction of movement just in case the diagonal we calculated above ends up
          // going through a wall. if the ray hits, we back off the horizontal movement to stay in bounds.
          var ray = isGoingRight ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;
          RaycastHit2D raycastHit;
          raycastHit = Physics2D.Raycast(ray, deltaMovement.normalized, deltaMovement.magnitude,
              platformMask & ~oneWayPlatformMask);

          if (raycastHit) {
            // we crossed an edge when using Pythagoras calculation, so we set the actual delta movement to the ray hit location
            deltaMovement = (Vector3) raycastHit.point - ray;
            if (isGoingRight)
              deltaMovement.x -= skinWidth;
            else
              deltaMovement.x += skinWidth;
          }

          isGoingUpSlope = true;
          collisionState.below = true;
        }
      } else // too steep. get out of here
      {
        deltaMovement.x = 0;
      }

      return true;
    }
    
    void moveVertically(ref Vector3 deltaMovement) {
      var isGoingUp = deltaMovement.y > 0;
      var rayDistance = Mathf.Abs(deltaMovement.y) + skinWidth;
      var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
      var initialRayOrigin = isGoingUp ? _raycastOrigins.topLeft : _raycastOrigins.bottomLeft;

      // apply our horizontal deltaMovement here so that we do our raycast from the actual position we would be in if we had moved
      initialRayOrigin.x += deltaMovement.x;

      // if we are moving up, we should ignore the layers in oneWayPlatformMask
      var mask = platformMask;
      if (isGoingUp)
        mask &= ~oneWayPlatformMask;

      for (var i = 0; i < totalVerticalRays; i++) {
        var ray = new Vector2(initialRayOrigin.x + i * horizontalDistanceBetweenRays, initialRayOrigin.y);

        DrawRay(ray, rayDirection * rayDistance, Color.blue);
        _raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, mask);
        if (_raycastHit) {
          // set our new deltaMovement and recalculate the rayDistance taking it into account
          if (!isGoingUp && (1 << _raycastHit.transform.gameObject.layer & oneWayPlatformMask) != 0) {
            var bounds = _raycastOrigins.bottomRight - _raycastOrigins.topLeft;
            var hit = Physics2D.OverlapBox(transform.position, bounds, 0, oneWayPlatformMask);
            if (hit) {
              break;
            }
          }

          deltaMovement.y = _raycastHit.point.y - ray.y;
          rayDistance = Mathf.Abs(deltaMovement.y);

          // remember to remove the skinWidth from our deltaMovement
          if (isGoingUp) {
            deltaMovement.y -= skinWidth;
          } else {
            deltaMovement.y += skinWidth;
            //Debug.Log("grounded in move vertical 2");
            collisionState.below = true;
          }

          // this is a hack to deal with the top of slopes. if we walk up a slope and reach the apex we can get in a situation
          // where our ray gets a hit that is less then skinWidth causing us to be ungrounded the next frame due to residual velocity.
          if (!isGoingUp && deltaMovement.y > 0.00001f)
            isGoingUpSlope = true;

          // we add a small fudge factor for the float operations here. if our rayDistance is smaller
          // than the width + fudge bail out because we have a direct impact
          if (rayDistance < skinWidth + kSkinWidthFloatFudgeFactor)
            break;
        }
      }
    }


    /// <summary>
    /// checks the center point under the BoxCollider2D for a slope. If it finds one then the deltaMovement is adjusted so that
    /// the player stays grounded and the slopeSpeedModifier is taken into account to speed up movement.
    /// </summary>
    /// <param name="deltaMovement">Delta movement.</param>
    private void handleVerticalSlope(ref Vector3 deltaMovement) {
      // slope check from the center of our collider
      var centerOfCollider = (_raycastOrigins.bottomLeft.x + _raycastOrigins.bottomRight.x) * 0.5f;
      var rayDirection = -Vector2.up;

      // the ray distance is based on our slopeLimit
      var slopeCheckRayDistance = slopeLimitTangent * (_raycastOrigins.bottomRight.x - centerOfCollider);

      var slopeRay = new Vector2(centerOfCollider, _raycastOrigins.bottomLeft.y);
      DrawRay(slopeRay, rayDirection * slopeCheckRayDistance, Color.yellow);
      _raycastHit = Physics2D.Raycast(slopeRay, rayDirection, slopeCheckRayDistance, platformMask);
      if (_raycastHit) {
        // bail out if we have no slope
        var angle = Vector2.Angle(_raycastHit.normal, Vector2.up);
        if (angle == 0) {
          return;
        }

        // we are moving down the slope if our normal and movement direction are in the same x direction
        var isMovingDownSlope = _raycastHit.normal.x * deltaMovement.x > 0;
        if (isMovingDownSlope) {
          // going down we want to speed up in most cases so the slopeSpeedMultiplier curve should be > 1 for negative angles
          var slopeModifier = slopeSpeedMultiplier.Evaluate(-angle);
          // we add the extra downward movement here to ensure we "stick" to the surface below
          deltaMovement.y += _raycastHit.point.y - slopeRay.y - skinWidth;
          deltaMovement = new Vector3(0, deltaMovement.y, 0) +
                          (Quaternion.AngleAxis(-angle, Vector3.forward) *
                           new Vector3(deltaMovement.x * slopeModifier, 0, 0));
        }
      }
    }
  }
}