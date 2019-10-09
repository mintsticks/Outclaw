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

    [Header("Turning")]
    [SerializeField] [Range(0, 360)] private float lookAngle = 30;
    [SerializeField] private float headTurnTime = .1f;
    [SerializeField] private float lookPause = .1f;
    [SerializeField] private float changeDirPause = .3333f;

    private int targetIdx = 0;
    private ManagedCoroutine patrolRoutine = null;

    public LineRenderer Path { get => path; }
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

      targetIdx = SnapToStart();
      if(path.positionCount > 1 && !movingLeft){
        ++targetIdx;
      }

      patrolRoutine = new ManagedCoroutine(this, Patrol);
    }

    public void StartPatrol(){
      patrolRoutine.StartCoroutine();
    }

    public void StopPatrol(){
      patrolRoutine.StopCoroutine();
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

      return initial ^ movingLeft;
    }

    private IEnumerator Patrol(){
      movement.UpdateVisionCone(movingLeft ? Vector3.left : Vector3.right);
      while(true){
        if((transform.position - path.GetPosition(targetIdx)).magnitude < arrivalTolerance){
          if(NextPoint()){
            yield return LookAround();
          }
        }
        movement.MoveTowards(path.GetPosition(targetIdx), Time.deltaTime);
        movement.UpdateVisionCone(movingLeft ? Vector3.left : Vector3.right);
        yield return new WaitForSeconds(0);
      }
    }

    private IEnumerator LookAround(){
      // inverted because it was toggle before this call
      Vector3 centerDir = !movingLeft ? Vector3.left : Vector3.right;
      Quaternion centerRot = Quaternion.LookRotation(Vector3.forward, centerDir);
      Quaternion left = centerRot * Quaternion.AngleAxis(lookAngle, Vector3.forward);
      Quaternion right = centerRot * Quaternion.AngleAxis(-lookAngle, Vector3.forward);

      yield return movement.TurnHead(movingLeft ? left : right, headTurnTime);
      yield return new WaitForSeconds(lookPause);
      yield return movement.TurnHead(movingLeft ? right : left, 2 * headTurnTime);
      yield return new WaitForSeconds(lookPause);
      yield return movement.TurnHead(centerRot, headTurnTime);

      movement.ToggleVision(false);
      yield return new WaitForSeconds(changeDirPause);
      movement.ToggleVision(true);
      yield break;
    }

    private void TogglePathVisibility(bool shown){
      path.enabled = shown;
    }
  }
}