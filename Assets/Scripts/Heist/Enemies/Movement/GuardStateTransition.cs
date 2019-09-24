﻿#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist{
  public class GuardStateTransition : MonoBehaviour
  {
    public enum GuardState { PATROL, CHASE, TRAVEL, INVESTIGATE }

    private GuardState state = GuardState.PATROL;

    [Header("Component Links")]
    [Inject(Id = "Vision Cone")] private GameObject visionCone;
    [SerializeField] private Pathfinder pathing;
    [SerializeField] private PatrolMovement patrol;
    [SerializeField] private ChaseMovement chase;
    [SerializeField] private InvestigateMovement investigate;

    void Start() {
      transform.position = patrol.NextDestination;
      StartPatrol();
    }

    private void EndCurrentMovement(){
      switch(state){
        case GuardState.PATROL:
          patrol.EndPatrol();
          break;
        case GuardState.CHASE:
          chase.EndChase();
          break;
        case GuardState.TRAVEL:
          pathing.StopMoving();
          pathing.OnArrival.RemoveAllListeners();
          break;
        case GuardState.INVESTIGATE:
          investigate.StopInvestigation();
          break;
      }
    }

    public void StartChase(GameObject target){
      EndCurrentMovement();
      state = GuardState.CHASE;
      chase.StartChase(target);
      visionCone.SetActive(false);
    }

    public void StartPatrol(){
      EndCurrentMovement();
      state = GuardState.TRAVEL;
      pathing.OnArrival.AddListener(TravelToPatrolCallback);
      pathing.GoTo(patrol.NextDestination);
      RecoverVision();
    }

    private void TravelToPatrolCallback(){
      pathing.OnArrival.RemoveListener(TravelToPatrolCallback);
      state = GuardState.PATROL;
      patrol.StartPatrol();
    }

    public void RecoverVision() {
      if(!visionCone.activeSelf){
        StartCoroutine(RestartVision());
      }
    }

    private IEnumerator RestartVision() {
      yield return null;
      visionCone.SetActive(true);
      yield break;
    }

    public void StartInvestigate(Vector3 location){
      EndCurrentMovement();
      state = GuardState.TRAVEL;
      pathing.OnArrival.AddListener(TravelToInvestigateCallback);
      pathing.GoTo(location);
      RecoverVision();
    }

    private void TravelToInvestigateCallback(){
      pathing.OnArrival.RemoveListener(TravelToInvestigateCallback);
      state = GuardState.INVESTIGATE;
      investigate.StartInvestigation();
    }
  }
}
