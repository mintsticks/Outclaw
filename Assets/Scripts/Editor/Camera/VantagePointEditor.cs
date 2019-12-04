using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Outclaw.Heist{
  [CustomEditor(typeof(VantagePoint))]
  public class VantagePointEditor : Editor
  {
    public void OnSceneGUI(){
      VantagePoint vp = (VantagePoint)target;

      Vector2 extent = new Vector2(Camera.main.aspect * vp.HalfCameraHeight, 
        vp.HalfCameraHeight);

      Vector2 lowerLeft = (Vector2)vp.CameraPosition + extent;
      Vector2 upperRight = (Vector2)vp.CameraPosition - extent;
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
