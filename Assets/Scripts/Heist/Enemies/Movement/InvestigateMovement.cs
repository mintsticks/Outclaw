#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Outclaw.Heist{
  public class InvestigateMovement : MonoBehaviour
  {
    public class OnMove : UnityEvent<Vector3>{}

    [Header("Searching Location")]
    [SerializeField] private float turnDuration;
    [Tooltip("Angle away from arrival direction. Total angle is double the value.")]
    [SerializeField] private float turnAngle;
    [SerializeField] private UnityEvent onInvestigationEnd;
    public UnityEvent OnInvestigationEnd { get => onInvestigationEnd; }

    [Header("Investigation Points")]
    [Tooltip("Number of samples to determine best search area.")]
    [SerializeField] private int numAreaSamples = 10;
    [SerializeField] private int numSearchs = 4;
    [SerializeField] private float minSearchDistance = 1;
    [SerializeField] private float maxSearchDistance = 7;

    // searching
    private List<Vector3> searchPoints = null;
    private int nextPoint = 0;
    private Coroutine searchRoutine = null;

    [SerializeField] private Pathfinder pathing;
    [Inject(Id = "Vision Cone")] private GameObject visionCone;
    [Inject(Id = "Raycast Layers")] private LayerMask hitLayers;

    private Coroutine investigateRoutine;

    public void StartInvestigation(){
      if(searchPoints == null){
        searchPoints = FindExplorePoints();
        MoveToNextPoint();
      }
    }

    public void StopInvestigation(){
      if(searchPoints != null){
        // stop pathing if active
        pathing.StopMoving();
        nextPoint = 0;
        searchPoints = null;

        // stop searching if active
        if(searchRoutine != null){
          StopCoroutine(searchRoutine);
          searchRoutine = null;
        }
      }
    }

    private void MoveToNextPoint(){
      Vector3 destination = searchPoints[nextPoint++];
      pathing.OnArrival.RemoveAllListeners();
      if(nextPoint < searchPoints.Count){
        pathing.OnArrival.AddListener(SearchPoint);
      }
      else{
        pathing.OnArrival.AddListener(FinishInvestigation);
      }
      pathing.GoTo(destination);
    }

    private void FinishInvestigation(){
      pathing.OnArrival.RemoveAllListeners();
      onInvestigationEnd.Invoke();
    }

    private void SearchPoint(){
      if(searchRoutine == null){
        searchRoutine = StartCoroutine(Search());
      }
    }

    private IEnumerator Search(){

      Quaternion original = transform.rotation;
      Quaternion left = original * Quaternion.AngleAxis(turnAngle, Vector3.forward);
      Quaternion right = original * Quaternion.AngleAxis(-turnAngle, Vector3.forward);
      Quaternion back = original * Quaternion.AngleAxis(180f, Vector3.forward);
      yield return TurnHead(left);
      yield return TurnHead(right);
      yield return TurnHead(left);
      yield return TurnHead(back);
      searchRoutine = null;
      MoveToNextPoint();
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

    private List<Vector3> FindExplorePoints(){
      List<Vector3> sample = new List<Vector3>();
      for(int i = 0; i < numAreaSamples; ++i){

        // cast ray to sample area
        float angle = 360f * i / numAreaSamples;
        Vector3 ray = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.up;
        ray.Normalize();
        RaycastHit2D hit = Physics2D.Raycast(transform.position,
          ray, Mathf.Infinity, hitLayers);

        float dist = maxSearchDistance;

        // hit, use dist if within max and min distance
        if(hit.collider != null){
          Vector3 toHit = (Vector3)hit.point - transform.position;
          float testDist = toHit.magnitude;
          
          // too close to the hit, ignore ray
          if(testDist < minSearchDistance){
            continue;
          }
          if(testDist < maxSearchDistance){
            dist = testDist;
          }
        }

        float searchDist = Mathf.Abs(RandomUtility.RandomStandardNormal());
        searchDist = (searchDist - dist) / (dist / 5);
        if(searchDist < minSearchDistance){
          searchDist = minSearchDistance;
        }
        sample.Add(transform.position + (ray * dist));
      }

      // pick numSearches points out of sample
      List<Vector3> res = new List<Vector3>();
      int startIndex = 0;
      for(int i = 0; i < numSearchs; ++i){
        int nextIndex = Random.Range(startIndex, 
          startIndex + ((sample.Count - startIndex) / (numSearchs - i)));
        res.Add(sample[nextIndex]);
        startIndex = nextIndex + 1;
      }
      RandomUtility.ShuffleList(res);

      return res;
    }
  }
}
