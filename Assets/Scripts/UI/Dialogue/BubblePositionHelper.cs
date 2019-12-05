using System;
using System.Collections.Generic;
using System.Linq;
using Outclaw;
using UnityEngine;
using Utility;

namespace UI.Dialogue {
  public class BubblePositionHelper : MonoBehaviour {
    [SerializeField] private int numPositions;
    [SerializeField] private float tailDistance;
    [SerializeField] private float headSize;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private BubbleTail bubbleTail;

    private List<Bounds> invalidBounds;
    private Camera camera;
    private Transform parent;
    private Vector3 parentCachedPos;
    private RectTransform bubbleImage;
    private Canvas canvas;
    private bool shouldUpdatePosition;

    public void Initialize(Vector3 inputPosition, Transform parent, Camera camera) {
      shouldUpdatePosition = false;
      transform.position = inputPosition;
      this.parent = parent;
      this.camera = camera;
      UpdateTail();
    }
    
    public void Initialize(
      List<Bounds> invalidBounds, 
      Camera camera, 
      Transform parent, 
      RectTransform bubbleImage, 
      Canvas canvas) {
      shouldUpdatePosition = true;
      this.invalidBounds = invalidBounds;
      this.camera = camera;
      this.parent = parent;
      this.bubbleImage = bubbleImage;
      this.canvas = canvas;
      UpdateComponent();
    }

    private void UpdateComponent() {
      UpdatePosition();
      UpdateTail();
      parentCachedPos = parent.position;
    }
    
    public void StopFollowing() {
      shouldUpdatePosition = false;
    }
    
    private void UpdatePosition() {
      FindValidPosition(90);
    }

    private void UpdateTail() {
      var position = parent.position;
      var bubblePos = camera.ScreenToWorldPoint(transform.position);
      var dirVector = (bubblePos- position).normalized;
      bubbleTail.UpdatePoints(position + headSize * dirVector,  bubblePos);
    }

    private void FindValidPosition(float startAngle) {
      var cameraBounds = camera.OrthographicBounds();
      cameraBounds.center = new Vector3(cameraBounds.center.x, cameraBounds.center.y, 0);
      
      for(var i = 0; i <= numPositions; i++) {
        var angle = GetAngleForIndex(i, GlobalConstants.CIRCLE_ANGLE / numPositions, startAngle);
        var pos = VectorUtil.GetPositionForAngle(parent.position, tailDistance + canvas.scaleFactor * GetPaddingForAngle(angle), angle);
        var newPos = camera.WorldToScreenPoint(pos);
        var bubbleBound = new Bounds(newPos, bubbleImage.sizeDelta).ScreenToWorld(camera).WithZ(0);
        var corners = new Vector3[4];
        bubbleImage.GetWorldCorners(corners);
        foreach (var corner in corners) {
          Debug.Log(corner);
        }
        Debug.Log(bubbleImage.sizeDelta);
        Debug.DrawLine(bubbleBound.min, bubbleBound.max, Color.white, 10);
        

        if (!bubbleBound.IsFullyInBounds(cameraBounds)) {
          Debug.DrawLine(pos, pos + new Vector3(1f, .1f, 1), Color.magenta, 5);
          continue;
        }

        if (invalidBounds.Any(bound => bubbleBound.Intersects(bound.WithZ(0)))) {
          Debug.DrawLine(pos, pos + new Vector3(1f, .1f, 1), Color.red, 5);
          continue;
        }
        
        Debug.DrawLine(pos, pos + new Vector3(1f, .1f, 1), Color.yellow, 5);
        transform.position = newPos;
        return;
      }
      
      var defaultPos = VectorUtil.GetPositionForAngle(parent.position, tailDistance + canvas.scaleFactor * GetPaddingForAngle(startAngle), startAngle);
      transform.position = camera.WorldToScreenPoint(defaultPos);
    }
    

    private float GetAngleForIndex(int index, float angleIncrement, float startAngle) {
      var angle = startAngle + (index % 2 * 2 - 1) * (index / 2) * angleIncrement;
      return angle;
    }
    
    private float GetPaddingForAngle(float angle) {
      var rad = Mathf.Deg2Rad * angle;
      var bubbleBound = new Bounds(Vector3.zero,  bubbleImage.sizeDelta).ScreenToWorld(camera);
      var bubbleSize = bubbleBound.extents;

      var widthDist = float.MaxValue;
      var cos = Mathf.Abs(Mathf.Cos(rad));
      if (cos >= .001f) {
        widthDist = bubbleSize.x / cos; 
      }
      
      var heightDist = float.MaxValue;
      var sin = Mathf.Abs(Mathf.Sin(rad));
      if (sin >= .001f) {
        heightDist = bubbleSize.y / sin; 
      }

      return Mathf.Min(widthDist, heightDist);
    }
  }
}