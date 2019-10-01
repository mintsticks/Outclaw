#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw.Heist{
  public class SideScrollPatrol : MonoBehaviour
  {

    [Header("Component Links")]
    [SerializeField] private LineRenderer path;
    [SerializeField] private GuardMovement movement;

    [Header("Movement")]
    [SerializeField] private LayerMask groundLayer;
    [Tooltip("Distance to a point before switching points.")]
    [SerializeField] private float arrivalTolerance = .1f;
    [Tooltip("Fraction of the total distance to start at.")]
    [SerializeField] [Range(0, 1)] private float startPosition;
    [SerializeField] private bool movingLeft;

    private int targetIdx;

    public LineRenderer Path { get => path; }
    public LayerMask GroundLayer { get => groundLayer; }

    public void Start(){
      InitPatrol();
      StartCoroutine(Patrol());
      TogglePathVisibility(false);
    }

    public void InitPatrol(){
      if(path.positionCount == 0){
        Debug.LogError("Not enough points on path.");
        return;
      }

      int targetIdx = SnapToStart();
      if(!movingLeft){
        ++targetIdx;
      }
    }

    // returns the index to the left of the starting position
    private int SnapToStart(){

      // nothing to compute
      if(path.positionCount == 1){
        transform.position = path.GetPosition(0);
        return 0;
      }

      // determine distance between each point
      List<float> distanceBetween = new List<float>();
      distanceBetween.Add(0);
      Vector3 prevPoint = path.GetPosition(0);
      for(int i = 1; i < path.positionCount; ++i){
        Vector3 nextPoint = path.GetPosition(i);
        distanceBetween.Add((nextPoint - prevPoint).magnitude);
        prevPoint = nextPoint;
      }

      // find the points to look between
      float startDist = distanceBetween[distanceBetween.Count - 1]
        * startPosition;
      int leftIdx = 0;
      while(leftIdx < distanceBetween.Count && distanceBetween[leftIdx] < startDist){
        ++leftIdx;
      }
      --leftIdx;

      // move to the starting position
      float distFromLeft = startDist - distanceBetween[leftIdx];
      transform.position = (Vector3.Normalize(path.GetPosition(leftIdx + 1) 
        - path.GetPosition(leftIdx)) * distFromLeft) + path.GetPosition(leftIdx);
      return leftIdx;

    }

    private void NextPoint(){
      // move point, turn around if at end
      if(movingLeft){
        --targetIdx;
        if(targetIdx < 0){
          ++targetIdx;
          movingLeft = false;
        }
      }
      else{
        ++targetIdx;
        if(targetIdx >= path.positionCount){
          --targetIdx;
          movingLeft = true;
        }

      }
    }

    private IEnumerator Patrol(){
      while(true){
        if((transform.position - path.GetPosition(targetIdx)).magnitude < arrivalTolerance){
          NextPoint();
        }
        movement.MoveTowards(path.GetPosition(targetIdx), Time.deltaTime);
        movement.UpdateVisionCone(movingLeft ? Vector3.left : Vector3.right);
        yield return new WaitForSeconds(0);
      }
    }

    private void TogglePathVisibility(bool shown){
      path.enabled = shown;
    }
  }
}