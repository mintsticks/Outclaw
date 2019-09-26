using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Outclaw.City{
  [CustomEditor(typeof(CameraBehavior))]
  public class CameraBehaviorEditor : Editor
  {
    void OnSceneGUI(){
      CameraBehavior cam = (CameraBehavior)target;

      Vector2 lowerLeft = cam.MinBound;
      Vector2 upperRight = cam.MaxBound;
      Vector2 upperLeft = new Vector2(lowerLeft.x, upperRight.y);
      Vector2 lowerRight = new Vector2(upperRight.x, lowerLeft.y);
      
      Handles.color = Color.red;
      Handles.DrawLine(lowerLeft, lowerRight);
      Handles.DrawLine(upperRight, lowerRight);
      Handles.DrawLine(upperRight, upperLeft);
      Handles.DrawLine(lowerLeft, upperLeft);
    }
  }
}
