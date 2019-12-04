#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Outclaw.ManagedRoutine;

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
    [Tooltip("Angle below forward to rotate vision.")]
    [SerializeField] [Range(0, 360)] private float visionAngle = 30;

    [Header("Turning")]
    [SerializeField] [Range(0, 360)] private float lookAngle = 30;
    [SerializeField] private float headTurnTime = .1f;
    [SerializeField] private float lookPause = .1f;

    private int targetIdx = 0;
    private ManagedCoroutine patrolRoutine = null;

    public LineRenderer Path { get => path; }

    public GuardMovement MovementComponent {
      get => movement; 
    }
    public LayerMask GroundLayer { get => groundLayer; }

    public void Start(){
      InitPatrol();
      StartPatrol();
      TogglePathVisibility(false);
    }

    public void InitPatrol(){
      if(path.positionCount == 0){
        Debug.LogError("Not enough points on path.");
        return;
      }

      // set to correct target point
      targetIdx = SnapToStart();
      if(path.positionCount > 1 && !movingLeft){
        ++targetIdx;
      }

      // set default vision
      ComputeVisionDirection(out Vector3 leftDir, out Vector3 rightDir);
      movement.UpdateVisionCone(movingLeft ? leftDir : rightDir);
      movement.SetArmAngle(visionAngle);

      patrolRoutine = new ManagedCoroutine(this, Patrol);
    }

    public void StartPatrol(){
      patrolRoutine.StartCoroutine();
    }

    public void StopPatrol(){
      patrolRoutine.StopCoroutine();
    }

    public void MoveToStart(){
      SnapToStart();
    }

    // returns the index to the left of the starting position
    private int SnapToStart(){

      // nothing to compute
      if(path.positionCount == 1){
        movement.transform.position = path.GetPosition(0);
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
      movement.transform.position = (Vector3.Normalize(path.GetPosition(leftIdx + 1) 
        - path.GetPosition(leftIdx)) * distFromLeft) + path.GetPosition(leftIdx);
      return leftIdx;

    }

    // returns true if the direction has changed
    private bool NextPoint(){
      if(path.positionCount <= 1){
        return false;
      }

      bool initial = movingLeft;

      // move point, turn around if at end
      if(movingLeft){
        --targetIdx;
        if(targetIdx < 0){
          targetIdx += 2;
          movingLeft = false;
        }
      }
      else{
        ++targetIdx;
        if(targetIdx >= path.positionCount){
          targetIdx -= 2;
          movingLeft = true;
        }
      }

      return initial ^ movingLeft;
    }

    private IEnumerator Patrol(){
      ComputeVisionDirection(out Vector3 leftDir, out Vector3 rightDir);
      movement.UpdateVisionCone(movingLeft ? leftDir : rightDir);
      while(true){
        if((movement.transform.position - path.GetPosition(targetIdx)).magnitude < arrivalTolerance){
          if(NextPoint()){
            yield return movement.Turn(leftDir, rightDir, movingLeft,
              lookPause, headTurnTime, visionAngle);
          }
        }
        movement.MoveTowards(path.GetPosition(targetIdx), Time.deltaTime);
        movement.UpdateVisionCone(movingLeft ? leftDir : rightDir);
        yield return new WaitForSeconds(0);
      }
    }

    private void ComputeVisionDirection(out Vector3 leftDir, 
        out Vector3 rightDir){

      leftDir = Quaternion.AngleAxis(visionAngle, Vector3.forward) 
        * Vector3.left;
      rightDir = Quaternion.AngleAxis(-visionAngle, Vector3.forward)
        * Vector3.right;
    }

    private void TogglePathVisibility(bool shown){
      path.enabled = shown;
    }
  }
}