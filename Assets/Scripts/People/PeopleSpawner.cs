using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw.City{
  public class PeopleSpawner : MonoBehaviour
  {
    [SerializeField] private List<GameObject> people;

    [Header("Spawning")]
    [SerializeField] private int numPeopleToSpawn;
    [SerializeField] private float spawnHeight;
    [SerializeField] private float distBeyondCamera;

    [Header("Movement")]
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float boundBeyondCamera;

    [Header("Animation")]
    [Tooltip("How much of the difference of speed to minSpeed should affect animation")]
    [SerializeField] private float ratioAnimInfluence = .5f;

    private float minPos;
    private float maxPos;
    private float minCamBound;
    private float maxCamBound;
    private Camera mainCam;

    void Start(){
      mainCam = Camera.main;
      InitBounds();
      RemoveInvalid();
      SpawnInitial();
    }

    void Update(){
      InitBounds();
    }

    private void SpawnInitial(){
      Vector3 spawnPos = new Vector3(0, spawnHeight, 0);
      for(int i = 0; i < numPeopleToSpawn; ++i){
        spawnPos.x = Mathf.Lerp(minPos, maxPos, Random.Range(0f, 1f));
        SpawnPerson(people[Random.Range(0, people.Count)], spawnPos);
      }
    }

    private void RemoveInvalid(){
      for(int i = people.Count - 1; i >= 0; --i){
        if(people[i].GetComponent<MoveAndLoop>() == null){
          Debug.LogWarning("Removed " + people[i] + " because it didn't have a MoveAndLoop component.");
          people.RemoveAt(i);
        }
      }
    }

    private void InitBounds(){
      Bounds bounds = mainCam.OrthographicBounds();
      minCamBound = bounds.min.x;
      maxCamBound = bounds.max.x;
      minPos = minCamBound - boundBeyondCamera;
      maxPos = maxCamBound + boundBeyondCamera;
    }

    private void SpawnPerson(GameObject person, Vector3 spawnPos, bool? moveLeft = null){
      float speed = Random.Range(minSpeed, maxSpeed);
      speed *= ((moveLeft != null) ? moveLeft.Value : (Random.Range(0, 2) == 0))
        ? -1 : 1;

      // spawn and init
      GameObject newPerson = Instantiate(person, spawnPos, Quaternion.identity, transform);
      MoveAndLoop component = newPerson.GetComponent<MoveAndLoop>();
      component.Init(this, speed, ((Mathf.Abs(speed) - minSpeed) * ratioAnimInfluence) + 1);
    }

    public bool IsInBounds(float x){
      return minPos < x && x < maxPos;
    }

    public void SpawnOutsideCamera(){
      GameObject person = people[Random.Range(0, people.Count)];
      float spawnDist = Random.Range(distBeyondCamera, boundBeyondCamera);
      bool spawnOnLeft = Random.Range(0, 2) == 0;
      float spawnX = spawnOnLeft ? minCamBound - spawnDist : maxCamBound + spawnDist;

      // if walk opposite direction of spawned direction
      SpawnPerson(person, new Vector3(spawnX, spawnHeight, 0), !spawnOnLeft);
    }
  }
}
