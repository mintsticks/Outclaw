using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw.City{
  public class PeopleSpawner : MonoBehaviour
  {
    [Header("People")]
    [SerializeField] private List<GameObject> people;
    [SerializeField] private int numPeopleToSpawn;

    [Header("Movement Params")]
    [SerializeField] private LineRenderer line;
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;

    private Vector3 minPos;
    private Vector3 maxPos;

    void Start(){
      if(line.positionCount != 2){
        Debug.LogError("LineRenderer must only have 2 points for the ends");
        return;
      }
      InitBounds();
      RemoveInvalid();
      for(int i = 0; i < numPeopleToSpawn; ++i){
        SpawnPerson(people[Random.Range(0, people.Count)]);
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
      Vector3 pos0 = line.GetPosition(0);
      Vector3 pos1 = line.GetPosition(1);

      minPos = new Vector3(
          Mathf.Min(pos0.x, pos1.x),
          Mathf.Min(pos0.y, pos1.y),
          Mathf.Min(pos0.z, pos1.z)
        );
      maxPos = new Vector3(
          Mathf.Max(pos0.x, pos1.x),
          Mathf.Max(pos0.y, pos1.y),
          Mathf.Max(pos0.z, pos1.z)
        );
    }

    private void SpawnPerson(GameObject person){
      float speed = Random.Range(minSpeed, maxSpeed);
      speed *= (Random.Range(0, 2) == 0)? -1 : 1;
      Vector3 spawnPos = Vector3.Lerp(minPos, maxPos, Random.Range(0f, 1f));

      // spawn and init
      GameObject newPerson = Instantiate(person, spawnPos, Quaternion.identity, transform);
      MoveAndLoop component = newPerson.GetComponent<MoveAndLoop>();
      component.Init(minPos.x, maxPos.x, speed);
    }
  }
}
