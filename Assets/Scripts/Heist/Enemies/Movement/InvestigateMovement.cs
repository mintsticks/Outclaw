#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

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
    [Tooltip("Number of samples to determine best search area.")]
    [SerializeField] private int numAreaSamples = 10;

    [Inject(Id = "Vision Cone")] private GameObject visionCone;
    [Inject(Id = "Raycast Layers")] private LayerMask hitLayers;

    private Coroutine investigateRoutine;

    public void StartInvestigation(){
      if(investigateRoutine == null){
        investigateRoutine = StartCoroutine(Investigate());
      }
    }

    public void StopInvestigation(){
      if(investigateRoutine != null){
        StopCoroutine(investigateRoutine);
        investigateRoutine = null;
      }
    }

    public IEnumerator Investigate(){

      float? idealAngle = SampleArea();
      Quaternion center = (idealAngle == null)
        ? visionCone.transform.rotation
        : Quaternion.AxisAngle(Vector3.forward, idealAngle.Value);

      Quaternion left = Quaternion.AngleAxis(turnAngle, Vector3.forward) * center;
      Quaternion right = Quaternion.AngleAxis(-turnAngle, Vector3.forward) * center;


      Debug.DrawRay(transform.position, left * Vector3.up, Color.red, 100f);
      Debug.DrawRay(transform.position, right * Vector3.up, Color.blue, 100f);

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
      Quaternion start = visionCone.transform.rotation;
      while(totalTime < turnDuration){
        totalTime += Time.deltaTime;
        visionCone.transform.rotation = Quaternion.Lerp(start, end, totalTime / turnDuration);
        yield return null;
      }
      yield break;
    }

    // returns null if evenly spread, otherwise the angle average weighted by distance
    private float? SampleArea(){
      Vector3 locationSum = Vector3.zero;
      float totalWeight = 0;

      for(int i = 0; i < numAreaSamples; ++i){
        // get the ray to shoot
        float angle = 360f * i / numAreaSamples;
        Vector3 ray = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.up;


        // cast and add local pos to sum
        RaycastHit2D hit = Physics2D.Raycast(transform.position,
          ray, Mathf.Infinity, hitLayers);
        if(hit.collider != null){
          Vector3 toHit = (Vector3)hit.point - transform.position;
          locationSum += toHit;
          totalWeight += toHit.magnitude;

          Debug.DrawRay(transform.position, toHit,
            Color.Lerp(Color.white, Color.black, (float)i/numAreaSamples), 100f);
        }
      }

      if(locationSum != Vector3.zero){
        //locationSum /= totalWeight;
        float aveAngle = Mathf.Atan2(locationSum.y, locationSum.x) * Mathf.Rad2Deg;
        return aveAngle;
      }
      return null;
    }
  }
}
