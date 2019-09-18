#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Outclaw.Heist{
  public class InvestigateMovement : MonoBehaviour
  {
    [SerializeField] private float turnDuration;
    [SerializeField] private int numTurns;
    [SerializeField] 
    [Tooltip("Angle away from arrival direction. Total angle is double the value.")]
    private float turnAngle;
    [SerializeField] private UnityEvent onInvestigationEnd;
    public UnityEvent OnInvestigationEnd { get => onInvestigationEnd; }

    [SerializeField] private Transform visionCone;

    private Coroutine investigateRoutine;

    public void StartInvestigation(Vector3 lookDir){
      if(investigateRoutine == null){
        investigateRoutine = StartCoroutine(Investigate(lookDir));
      }
    }

    public void StopInvestigation(){
      if(investigateRoutine != null){
        StopCoroutine(investigateRoutine);
        investigateRoutine = null;
      }
    }

    public IEnumerator Investigate(Vector3 lookDir){

      Quaternion center = Quaternion.identity;
      center.SetLookRotation(Vector3.forward, lookDir);
      Quaternion left = Quaternion.AngleAxis(turnAngle, Vector3.forward) * visionCone.rotation;
      Quaternion right = Quaternion.AngleAxis(-turnAngle, Vector3.forward) * visionCone.rotation;
      for(int i = 0; i < numTurns; ++i){
        if(i % 2 == 0){
          yield return TurnHead(right);
        }
        else{
          yield return TurnHead(left);
        }
      }
      investigateRoutine = null;
      onInvestigationEnd.Invoke();
      yield break;
    }

    private IEnumerator TurnHead(Quaternion end){
      float totalTime = 0;
      Quaternion start = visionCone.rotation;
      while(totalTime < turnDuration){
        totalTime += Time.deltaTime;
        visionCone.rotation = Quaternion.Lerp(start, end, totalTime / turnDuration);
        yield return null;
      }
      yield break;
    }
  }
}
