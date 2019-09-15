using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw.Heist{
	public class PatrolMovement : MonoBehaviour
	{
		[SerializeField] private Transform waypointParent = null;
		[SerializeField] private float speed = 5;
		[SerializeField] private float arrivalTolerance = 3;

		[SerializeField]
		[Range(0, 1)]
		private float turnSpeed = .5f;
		private float turnTime = 1f;

		[SerializeField] private float visionRecoverTime = 1f;
		[SerializeField] private GameObject visionCone = null;

		private Coroutine patrolRoutine = null;

	    // Update is called once per frame
	    void Start()
	    {
	    	transform.position = waypointParent.GetChild(0).position;
	    	StartPatrol();
	    }

	    public void StartPatrol(){
	    	if(waypointParent.childCount > 0 && patrolRoutine == null){
		        patrolRoutine = StartCoroutine(Patrol());
		    }
	    }

	    public void EndPatrol(){
	    	if(patrolRoutine != null){
	    		StopCoroutine(patrolRoutine);
	    		patrolRoutine = null;
	    	}
	    }

	    private IEnumerator Patrol(){

	    	int currentGoal = 0;
	    	while(true){
				Vector2 dir = waypointParent.GetChild(currentGoal).position - transform.position;

				bool changedDir = false;
		        while(dir.magnitude < arrivalTolerance){
		        	currentGoal = (currentGoal + 1) % waypointParent.childCount;
		        	dir = waypointParent.GetChild(currentGoal).position - transform.position;
		        	changedDir = true;
		        }
		        if(changedDir){
					yield return TurnHead(dir);
		        }

		        dir.Normalize();
		        transform.Translate(dir * speed * Time.deltaTime);
		        yield return null;
	    	}
	    }

	    private IEnumerator TurnHead(Vector3 dir){

	    	float timePassed = 0;
	    	while(timePassed < turnTime){
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

	    public void RecoverVision(GameObject visionCone){
	    	StartCoroutine(RestartVision(visionCone));
	    }

	    private IEnumerator RestartVision(GameObject visionCone){
	    	yield return new WaitForSeconds(visionRecoverTime);
	    	visionCone.SetActive(true);
	    	yield break;
	    }
	}
}
