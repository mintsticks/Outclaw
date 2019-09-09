using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Outclaw.Heist{
	public class VisionCone : MonoBehaviour
	{
		private MeshFilter filter;
		[SerializeField]
		[Range(0, 360)]	
		private float coneAngle = 90;
		[SerializeField]
		private float visionDistance = 1;
		[SerializeField]
		private int numSamples = 10;

		public UnityEvent onDetect;

	    // Start is called before the first frame update
	    void Start()
	    {
	        filter = GetComponent<MeshFilter>();
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        if(TestCone()){
	        	onDetect.Invoke();
	        }
	    }

	    // returns if the player was found
	    private bool TestCone(){

	    	bool foundPlayer = false;
	    	List<Vector3> meshVerts = new List<Vector3>();
	    	meshVerts.Add(Vector3.zero);

	    	Vector3 coneStart = Quaternion.AngleAxis(-coneAngle/2, Vector3.forward) 
	    		* transform.up;
	    	coneStart.Normalize();

	    	Quaternion rotInverse = Quaternion.Inverse(transform.rotation);
	    	for(int i = 0; i < numSamples; ++i){

	    		// rotate
	    		Vector3 currVec = Quaternion.AngleAxis(coneAngle * i / numSamples, Vector3.forward) 
	    			* coneStart;
	    		currVec.Normalize();

	    		// cast
	    		RaycastHit2D hit = Physics2D.Raycast(transform.position,
	    			currVec, visionDistance);

	    		// move points to local space
	    		Vector3 localLineEnd;
	    		if(hit.collider == null){ // no hit, draw end of vision
	    			localLineEnd = currVec * visionDistance;
	    		}
	    		else{ // hit, draw where it hit
	    			localLineEnd = hit.point - (Vector2)transform.position;

	    			if(!foundPlayer && hit.collider.gameObject.CompareTag("Player")){
	    				foundPlayer = true;
	    			}
	    		}
    			// meshVerts.Add(rotInverse * localLineEnd);
    			meshVerts.Add(rotInverse * localLineEnd);

	    	}

	    	// create mesh
	    	Mesh m = filter.mesh;
	    	m.Clear();
	    	m.vertices = meshVerts.ToArray();
	    	List<int> tris = new List<int>();
	    	for(int i = 2; i < m.vertices.Length; ++i){
	    		tris.Add(0);
	    		tris.Add(i);
	    		tris.Add(i - 1);
	    	}
	    	m.triangles = tris.ToArray();

	    	return foundPlayer;
	    }
	}
}