using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw.Heist{
  public class SideScrollSentry : MonoBehaviour
  {
    [Header("Turning")]
    [Tooltip("Does this guard turn around?")]
    [SerializeField] private bool turns = true;
    [SerializeField] private float turnPause = 1f;
    [SerializeField] private float lookPause = .1f;
    [SerializeField] private float armTurnTime = 1f;

    [Header("Directions")]
    [Tooltip("The angle below forward to have the arm")]
    [SerializeField] [Range(0, 360)] private float armAngle = 30f;
    [SerializeField] private bool isFacingLeft;

    [Header("Component Links")]
    [SerializeField] private GuardMovement movement;

    void Start(){
      if(!isFacingLeft){
        movement.TurnBody();
      }

      ComputeVisionDirection(out Vector3 leftDir, out Vector3 rightDir);
      movement.UpdateVisionCone(isFacingLeft ? leftDir : rightDir);
      movement.SetArmAngle(armAngle);
      if (!turns) {
        return;
      }
      StartCoroutine(Sentry(leftDir, rightDir));
    }

    private IEnumerator Sentry(Vector3 leftDir, Vector3 rightDir){
      while(true){
        for(float time = 0; time < turnPause; time += Time.deltaTime){
          movement.UpdateVisionCone(isFacingLeft ? leftDir : rightDir);
          yield return null;
        }
        yield return LookAround(leftDir, rightDir);
      }
    }

    private void ComputeVisionDirection(out Vector3 leftDir, 
        out Vector3 rightDir){

      leftDir = Quaternion.AngleAxis(armAngle, Vector3.forward) 
        * Vector3.left;
      rightDir = Quaternion.AngleAxis(-armAngle, Vector3.forward)
        * Vector3.right;
    }

    private IEnumerator LookAround(Vector3 leftDir, Vector3 rightDir){
      movement.MoveTowards(movement.transform.position, 0);

      Vector3 endDir = isFacingLeft ? rightDir : leftDir;
      Quaternion bottomRot = Quaternion.AngleAxis(180f, Vector3.forward);
      Quaternion endRot =  Quaternion.LookRotation(Vector3.forward, endDir);
      yield return new WaitForSeconds(lookPause);
      yield return movement.TurnVision(bottomRot, armTurnTime, armAngle, false);
      movement.TurnBody();
      yield return movement.TurnVision(endRot, armTurnTime, armAngle, true);

      isFacingLeft = !isFacingLeft;
      yield break;
    }
  }
}
