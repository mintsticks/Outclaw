using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw.Heist{
  public class SideScrollSentry : MonoBehaviour
  {
    [Header("Turning")]
    [Tooltip("Does this guard turn around?")]
    [SerializeField] private bool turns = true;
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
      movement.SetArmAngle(armAngle);
      // both turning and nonturning need to keep updated the cone angle
      //  because the arm animation will pu ti tat the wrong angle
      StartCoroutine(Sentry(leftDir, rightDir));
    }

    private IEnumerator Sentry(Vector3 leftDir, Vector3 rightDir){
      while(true){
        if (!turns) {
          yield break;
        }
        yield return movement.Turn(leftDir, rightDir, !isFacingLeft, lookPause, 
          armTurnTime, armAngle, ToggleFacingDir);
      }
    }

    private void ComputeVisionDirection(out Vector3 leftDir, 
        out Vector3 rightDir){

      leftDir = Quaternion.AngleAxis(armAngle, Vector3.forward) 
        * Vector3.left;
      rightDir = Quaternion.AngleAxis(-armAngle, Vector3.forward)
        * Vector3.right;
    }

    private void ToggleFacingDir(){
      isFacingLeft = !isFacingLeft;
    }
  }
}
