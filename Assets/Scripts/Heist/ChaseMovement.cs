using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Outclaw.Heist{
	public class ChaseMovement : MonoBehaviour
	{
		[SerializeField] private float speed;
	    public GameObject target;
	    [SerializeField] private LayerMask hitLayers;
	    private Coroutine chaseRoutine;

	    public UnityEvent onTargetLost;

	    public void StartChase(GameObject target){

	    }

	    public void EndChase(){
	    	if(chaseRoutine != null){
	    		StopCoroutine(chaseRoutine);
	    		chaseRoutine = null;
	    		target = null;
	    	}
	    }

	    private IEnumerator Chase(){

	    	while(true){
	    		Vector3 toPlayer = target.transform.position - transform.position;
	    		RaycastHit2D hit = Physics2D.Raycast(transform.position,
	    			toPlayer, toPlayer.magnitude, hitLayers);

	    		if(hit.collider.gameObject.CompareTag("Player")){
		        	transform.Translate(Vector3.Normalize(toPlayer) * speed * Time.deltaTime);
	    		}
	    		else{
	    			chaseRoutine = null;
	    			target = null;
	    			yield break;
	    		}

	    		yield return null;
	    	}
	    }
	}
}