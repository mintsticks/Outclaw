using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SenseAbility : MonoBehaviour
{
	// actual effect
    [SerializeField] private GameObject visionField = null;
    private float origScale = 0f;
    [SerializeField] private float expandTime = 1f;
    [SerializeField] private float holdTime = 2f;
    [SerializeField] private float contractTime = 3f;
    [SerializeField] private float maxSizeScale = 3f;

    // using
    [SerializeField] private float cooldown = 10f;
    public bool Useable {get; private set;}

    void Start(){
    	origScale = visionField.transform.localScale.x;
    	Useable = true;
    }

    public void UseAbility(){
    	if(Useable){
    		StartCoroutine(Activate());
    		StartCoroutine(Cooldown());
    	}
    }

    private IEnumerator Cooldown(){
    	Useable = false;
    	yield return new WaitForSeconds(cooldown);
    	Useable = true;
    	yield break;
    }

    private IEnumerator Activate(){
    	yield return TransitionScale(1, maxSizeScale, expandTime);
    	yield return new WaitForSeconds(holdTime);
    	yield return TransitionScale(maxSizeScale, 1, contractTime);
    }

    private IEnumerator TransitionScale(float start, float end, float duration){
    	float timePassed = 0;
    	while(timePassed < duration){
    		timePassed += Time.deltaTime;
    		float scale = origScale * Mathf.Lerp(start, end, timePassed / duration);
    		visionField.transform.localScale = new Vector3(scale, scale, scale);
    		yield return null;
    	}

    	yield break;
    }
}
