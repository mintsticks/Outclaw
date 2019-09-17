#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw.Heist{
  public class GuardStateTransition : MonoBehaviour
  {
    public enum GuardState { PATROL, CHASE, RETURN }

    [SerializeField] private GuardState state;

    [Header("Component Links")]
    [SerializeField] private GameObject visionCone;
    [SerializeField] private Pathfinder pathing;
    [SerializeField] private PatrolMovement patrol;
    [SerializeField] private ChaseMovement chase;

    private void EndCurrentMovement(){
      switch(state){
        case GuardState.PATROL:
          patrol.EndPatrol();
          break;
        case GuardState.CHASE:
          chase.EndChase();
          break;
        case GuardState.RETURN:
          pathing.StopMoving();
          pathing.OnArrival.RemoveAllListeners();
          break;
      }
    }

    public void StartChase(GameObject target){
      EndCurrentMovement();
      chase.StartChase(target);
      visionCone.SetActive(false);
      state = GuardState.CHASE;
    }

    public void StartPatrol(){
      EndCurrentMovement();
      pathing.GoTo(patrol.NextDestination);
      patrol.RecoverVision(visionCone);
      pathing.OnArrival.AddListener(ReturnToPatrolCallback);
      state = GuardState.RETURN;
    }

    private void ReturnToPatrolCallback(){
      pathing.OnArrival.RemoveListener(ReturnToPatrolCallback);
      patrol.StartPatrol();
      state = GuardState.PATROL;
    }
  }
}
