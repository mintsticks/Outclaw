#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public class PatrolMovement : MonoBehaviour {
    [Header("Movement")]
    [SerializeField] private Transform waypointParent = null;
    [Inject(Id = "Passive Speed")] private float speed;
    [SerializeField] private float arrivalTolerance = 3;
    private int currentGoal = 0;

    [Header("Turning")]
    [SerializeField]
    [Range(0, 1)]
    private float turnSpeed = .5f;
    private float turnTime = 1f;

    [Header("Vision")]
    [SerializeField] private GameObject visionCone = null;

    private Coroutine patrolRoutine = null;

    public Vector3 NextDestination{
      get{
        return (waypointParent.childCount > currentGoal) 
          ? waypointParent.GetChild(currentGoal).position
          : transform.position;
      }
    }

    public void StartPatrol() {
      if (waypointParent.childCount > 0 && patrolRoutine == null) {
        patrolRoutine = StartCoroutine(Patrol());
      }
    }

    public void EndPatrol() {
      if (patrolRoutine != null) {
        StopCoroutine(patrolRoutine);
        patrolRoutine = null;
      }
    }

    private IEnumerator Patrol() {
      while (true) {
        Vector2 dir = waypointParent.GetChild(currentGoal).position - transform.position;

        bool changedDir = false;
        while (dir.magnitude < arrivalTolerance) {
          currentGoal = (currentGoal + 1) % waypointParent.childCount;
          dir = waypointParent.GetChild(currentGoal).position - transform.position;
          changedDir = true;
        }

        if (changedDir) {
          yield return TurnHead(dir);
        }

        dir.Normalize();
        transform.Translate(dir * speed * Time.deltaTime);
        yield return null;
      }
    }

    private IEnumerator TurnHead(Vector3 dir) {
      float timePassed = 0;
      while (timePassed < turnTime) {
        Quaternion oldRot = visionCone.transform.rotation;
        Quaternion goal = Quaternion.identity;
        goal.SetLookRotation(Vector3.forward, dir);
        visionCone.transform.rotation = Quaternion.Lerp(
          oldRot,
          goal,
          turnSpeed
        );
        timePassed += Time.deltaTime;
        yield return null;
      }

      yield break;
    }
  }
}