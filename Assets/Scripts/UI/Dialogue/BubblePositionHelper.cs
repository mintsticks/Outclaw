﻿using System;
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
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private BubbleTail bubbleTail;

    private List<Bounds> invalidBounds;
    private Camera camera;
    private Transform parent;
    private Vector3 parentCachedPos;
    private RectTransform bubbleImage;
    private float cachedAngle;
    
    public void Initialize(List<Bounds> invalidBounds, Camera camera, Transform parent, RectTransform bubbleImage) {
      this.invalidBounds = invalidBounds;
      this.camera = camera;
      this.parent = parent;
      this.bubbleImage = bubbleImage;
      canvas.alpha = 0;
    }

    public void Update() {
      if (parentCachedPos == parent.position) {
        return;
      }

      canvas.alpha = 1;
      UpdatePosition();
      UpdateTail();
      parentCachedPos = parent.position;
    }

    private void UpdatePosition() {
      if (invalidBounds.Count == 0) {
        FindValidPosition(90);
        return;
      }

      var interactableCenter = invalidBounds[0].center;
      var startAngle = Vector3.SignedAngle(Vector3.right, interactableCenter - parent.position, Vector3.forward);
      FindValidPosition(startAngle);
    }

    private void UpdateTail() {
      var position = parent.position;
      var dirVector = (camera.ScreenToWorldPoint(transform.position) - position).normalized;
      bubbleTail.UpdatePoints(position + headSize * dirVector,  position + tailDistance * dirVector);
    }

    private void FindValidPosition(float startAngle) {
      var cameraBounds = camera.OrthographicBounds();
      cameraBounds.center = new Vector3(cameraBounds.center.x, cameraBounds.center.y, 0);
      for(var i = 0; i <= numPositions; i++) {
        var angle = GetAngleForIndex(i, GlobalConstants.CIRCLE_ANGLE / numPositions, startAngle);
        var pos = VectorUtil.GetPositionForAngle(parent.position, tailDistance + GetPaddingForAngle(angle), angle);
        var newPos = camera.WorldToScreenPoint(pos);
        var bubbleBound = new Bounds(newPos, bubbleImage.sizeDelta).ScreenToWorld(camera);

        if (!bubbleBound.IsFullyInBounds(cameraBounds)) {
          continue;
        }

        if (invalidBounds.Any(bound => bubbleBound.Intersects(bound))) {
          continue;
        }

        cachedAngle = angle;
        transform.position = newPos;
        return;
      }
      
      var defaultPos = VectorUtil.GetPositionForAngle(parent.position, tailDistance + GetPaddingForAngle(startAngle), startAngle);
      transform.position = camera.WorldToScreenPoint(defaultPos);
      cachedAngle = startAngle;
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