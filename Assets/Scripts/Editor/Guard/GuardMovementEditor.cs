using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Outclaw.Heist{
  [CustomEditor(typeof(GuardMovement))]
  public class GuardMovementEditor : Editor
  {
    public void OnSceneGUI(){
      GuardMovement movement = (GuardMovement)target;

      Vector2 bottomLeft = movement.transform.TransformPoint(movement.BodyBounds.min);
      Vector2 topRight = movement.transform.TransformPoint(movement.BodyBounds.max);
      Vector2 topLeft = new Vector2(bottomLeft.x, topRight.y);
      Vector2 bottomRight = new Vector2(topRight.x, bottomLeft.y);

      Handles.color = Color.green;
      Handles.DrawLine(bottomLeft, topLeft);
      Handles.DrawLine(bottomLeft, bottomRight);
      Handles.DrawLine(topRight, topLeft);
      Handles.DrawLine(topRight, bottomRight);
    }
  }
}
