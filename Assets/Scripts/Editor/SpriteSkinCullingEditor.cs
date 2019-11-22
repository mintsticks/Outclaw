using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Outclaw{
  [CustomEditor(typeof(SpriteSkinCulling))]
  public class SpriteSkinCullingEditor : Editor
  {
    void OnSceneGUI(){
      SpriteSkinCulling culling = (SpriteSkinCulling)target;

      Vector2 lowerLeft = culling.transform.TransformPoint(culling.VisualBounds.min);
      Vector2 upperRight = culling.transform.TransformPoint(culling.VisualBounds.max);
      Vector2 upperLeft = new Vector2(lowerLeft.x, upperRight.y);
      Vector2 lowerRight = new Vector2(upperRight.x, lowerLeft.y);
      
      Handles.color = Color.yellow;
      Handles.DrawLine(lowerLeft, lowerRight);
      Handles.DrawLine(upperRight, lowerRight);
      Handles.DrawLine(upperRight, upperLeft);
      Handles.DrawLine(lowerLeft, upperLeft);
    }
  }
}
