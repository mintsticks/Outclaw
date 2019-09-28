using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Outclaw.Heist{
  [CustomEditor(typeof(SideScrollPatrol))]
  public class SideScrollPatrolEditor : Editor
  {
    private const int SAMPLES_BETWEEN_POINTS = 10;
    private const int NUM_PASSES = 3;
    private const float SIMPLIFY_TOL = .1f; 

    public override void OnInspectorGUI(){
      DrawDefaultInspector();
      if(GUILayout.Button("Snap Path to Ground")){
        for(int i = 0; i < NUM_PASSES; ++i){
          SnapToGround();
        }
      }
    }

    // if a raycast downwards hit the ground, returns the point height above the hit
    // otherwise, returns the point
    private Vector3 AdjustPoint(Vector3 testPoint, LayerMask layers, float height){
      // find nearest point to the ground
      RaycastHit2D hit = Physics2D.Raycast(testPoint,
          Vector3.down, Mathf.Infinity, layers);

      // no hit, leave point alone
      if(hit.collider == null){
        return testPoint;
      }

      // move point to be exactly half of height off of ground
      return (Vector3)hit.point + (Vector3.up * height);
    }

    private void SnapToGround(){
      SideScrollPatrol patrol = (SideScrollPatrol)target;
      LineRenderer path = patrol.Path;
      Collider2D col = patrol.gameObject.GetComponent<Collider2D>();

      float halfHeight = col.bounds.extents.y;

      Undo.RecordObject(patrol.Path, "Snapped path");
      EditorUtility.SetDirty(path);

      List<Vector3> newPoints = new List<Vector3>();
      for(int i = 0; i < path.positionCount - 1; ++i){
        Vector3 start = path.GetPosition(i);
        Vector3 direction = path.GetPosition(i + 1) - start;
        float distance = direction.magnitude;
        direction.Normalize();

        for(int j = 0; j < SAMPLES_BETWEEN_POINTS; ++j){

          Vector3 testPoint = start + (direction * distance 
            * j / SAMPLES_BETWEEN_POINTS);

          newPoints.Add(AdjustPoint(testPoint, patrol.GroundLayer,
            halfHeight));
        }
      }
      newPoints.Add(AdjustPoint(path.GetPosition(path.positionCount - 1),
        patrol.GroundLayer, halfHeight));

      path.positionCount = newPoints.Count;
      path.SetPositions(newPoints.ToArray());
      path.Simplify(SIMPLIFY_TOL);
    }
  }
}