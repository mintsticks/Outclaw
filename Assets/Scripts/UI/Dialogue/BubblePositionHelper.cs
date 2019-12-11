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
    [SerializeField] private BubbleTail bubbleTail;
    [SerializeField] private float defaultAngle = 90;
    
    private List<Bounds> invalidBounds;
    private Camera camera;
    private Transform parent;
    private Vector3 bubblePos;
    private RectTransform bubbleImage;
    private Canvas canvas;
    
    public void Initialize(List<Bounds> invalidBounds, Camera camera, Transform parent, RectTransform bubbleImage, Canvas canvas, Vector3? initialPosition = null) {
      this.invalidBounds = invalidBounds;
      this.camera = camera;
      this.parent = parent;
      this.bubbleImage = bubbleImage;
      this.canvas = canvas;
      InitializePosition(initialPosition);
    }
    
    private void InitializePosition(Vector3? inputPosition) {
      if (inputPosition == null || !BubblePositionValid(inputPosition.Value)) {
        transform.position = FindValidPosition(defaultAngle);
        UpdateTail();
        return;
      }
      transform.position = inputPosition.Value;
      UpdateTail();
    }
    
    private void UpdateTail() {
      var position = parent.position;
      var bubblePos = camera.ScreenToWorldPoint(transform.position);
      var dirVector = (bubblePos- position).normalized;
      bubbleTail.UpdatePoints(position + headSize * dirVector,  bubblePos);
    }

    private bool BubblePositionValid(Vector3 position) {
      var cameraBounds = camera.OrthographicBounds();
      cameraBounds.center = new Vector3(cameraBounds.center.x, cameraBounds.center.y, 0);
      var bubbleBound = new Bounds(position, canvas.scaleFactor * bubbleImage.sizeDelta).ScreenToWorld(camera).WithZ(0);
      if (!bubbleBound.IsFullyInBounds(cameraBounds)) {
        Debug.DrawLine(bubbleBound.min, bubbleBound.max, Color.cyan, 10);
        return false;
      }

      if (invalidBounds.Any(bound => bubbleBound.Intersects(bound.WithZ(0)))) {
        Debug.DrawLine(bubbleBound.min, bubbleBound.max, Color.cyan, 10);
        return false;
      }

      Debug.DrawLine(bubbleBound.min, bubbleBound.max, Color.green, 10);
      return true;
    }
    
    private Vector3 FindValidPosition(float startAngle) {
      for(var i = 0; i <= numPositions; i++) {
        var angle = GetAngleForIndex(i, GlobalConstants.CIRCLE_ANGLE / numPositions, startAngle);
        var pos = VectorUtil.GetPositionForAngle(parent.position, tailDistance +  GetPaddingForAngle(angle), angle);
        var newPos = camera.WorldToScreenPoint(pos);
        if (!BubblePositionValid(newPos)) {
          continue;
        }
        
        return newPos;
      }
      
      var defaultPos = VectorUtil.GetPositionForAngle(parent.position, tailDistance + GetPaddingForAngle(startAngle), startAngle);
      return camera.WorldToScreenPoint(defaultPos);
    }
    

    private float GetAngleForIndex(int index, float angleIncrement, float startAngle) {
      var angle = startAngle + (index % 2 * 2 - 1) * (index / 2) * angleIncrement;
      return angle;
    }
    
    private float GetPaddingForAngle(float angle) {
      var rad = Mathf.Deg2Rad * angle;
      var bubbleBound = new Bounds(Vector3.zero,  canvas.scaleFactor * bubbleImage.sizeDelta).ScreenToWorld(camera);
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