using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

using Zenject;
/*
 *  Using tilemap
 *   - Tilemap.cellBounds gives the cell coord for min-1 and max+1 also provide iterator over all cells
 *   - Tilemap.origin gives min-1 coord
 *   - Tilemap.GetCellCenterWorld(cell coord) gives world position for center of a given cell 
 *   - Tilemap.GetTile(cell coord) gives actual tile at coord, null if empty
 */

namespace Outclaw.Heist{
	public class Pathfinder : MonoBehaviour
	{
    public float speed = 5;
    private List<Vector3Int> path = null;
    private Coroutine pathRoutine = null;
    [SerializeField]
    [Range(0, 1)]
    private float turnSpeed = .5f;

    [SerializeField] private UnityEvent onArrival = new UnityEvent();
    public UnityEvent OnArrival { get => onArrival; }

    [Header("Other Components")]
    [SerializeField] private Tilemap map = null;
    [SerializeField] private Transform destination = null;
    [Inject(Id = "Vision Cone")] private GameObject visionCone;

    /*
     *  Debug Functions
     */
    private void DrawDebug(){
    	DrawCellCrosshair(NearestCell(transform.position), Color.white);
    	//DrawCellCrosshair(NearestCell(destination.position), Color.black);

    	if(path != null){
	    	foreach(Vector3Int cell in path){
	    		DrawCellCrosshair(cell, Color.blue);
	    	}
    	}
    }

    private void DrawCellCrosshair(Vector3Int cellCoord, Color color){
    	Debug.DrawRay(map.GetCellCenterWorld(cellCoord), Vector3.up / 2, color);
    	Debug.DrawRay(map.GetCellCenterWorld(cellCoord), Vector3.right / 2, color);
    }

    public void Go(){
    	if(path != null){
	    	if(pathRoutine != null){
	    		StopMoving();
	    	}
	    	StartMoving();
    	}
    }

    /*
     *  Pathfinding Functions
     */

    public void ComputePath(){
    	path = ShortestPath(
    			NearestCell(transform.position),
    			NearestCell(destination.position)
    		);
    }

    // returns the cell closest to a given point
    private Vector3Int NearestCell(Vector3 point){
    	return new Vector3Int(
    			(int)Mathf.Floor(point.x / map.layoutGrid.cellSize.x),
    			(int)Mathf.Floor(point.y / map.layoutGrid.cellSize.y),
    			0
    		);
    }

		private static readonly Vector3Int[] offsets = new Vector3Int[]{
			new Vector3Int(1, 0, 0), 
			new Vector3Int(0, 1, 0), 
			new Vector3Int(-1, 0, 0), 
			new Vector3Int(0, -1, 0)};

	  private List<Vector3Int> ShortestPath(Vector3Int start, Vector3Int end){

	  	// initial point
	  	Dictionary<Vector3Int, Tuple<int, Vector3Int>> cellCurrentBest = new
	  		Dictionary<Vector3Int, Tuple<int, Vector3Int>>();
	  	cellCurrentBest.Add(start, new Tuple<int, Vector3Int>(0, start));
	  	SortedDictionary<int, Stack<Vector3Int>> toVisit = new 
	  		SortedDictionary<int, Stack<Vector3Int>>();
	  	toVisit.Add(0, new Stack<Vector3Int>());
	  	toVisit[0].Push(start);

	  	// progress tracking
	  	HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
	  	bool found = false;
	  	BoundsInt bounds = map.cellBounds;
	  	int maxSearch = bounds.size.x * bounds.size.y;

	  	while(toVisit.Count > 0 && visited.Count < maxSearch){
	  		Vector3Int current = GetMinCell(toVisit);
	  		visited.Add(current);

	  		if(current == end){
	  			found = true;
	  			break;
	  		}


	  		foreach(Vector3Int offset in offsets){
	  			Vector3Int neighbor = current + offset;
	  			if(visited.Contains(neighbor) || map.GetTile(neighbor) != null){
	  				continue;
	  			}

					int cost = cellCurrentBest[current].Item1 + 1;

					if(cellCurrentBest.ContainsKey(neighbor) && cellCurrentBest[neighbor].Item1 <= cost){
						continue;
					}
					cellCurrentBest[neighbor] = new Tuple<int, Vector3Int>(cost, current);

					// prioritize by min cost to travel + min distance to go
					int weightedCost = cost + Distance(neighbor, end) + (IsNearWall(neighbor) ? 3 : 0);
					if(!toVisit.ContainsKey(weightedCost)){
						toVisit[weightedCost] = new Stack<Vector3Int>();
					}
					toVisit[weightedCost].Push(neighbor);
	    		}

	    	}

	    	List<Vector3Int> res = null;
	    	if(found){
	    		res = new List<Vector3Int>();
	    		Vector3Int current = end;
	    		while(current != start){
	    			res.Add(current);
	    			current = cellCurrentBest[current].Item2;
	    		}
	    		res.Reverse();
	    	}

    	return res;
    }

    private bool IsNearWall(Vector3Int pos){
    	foreach(Vector3Int offset in offsets){
  			if(map.GetTile(pos + offset) != null){
  				return true;
  			}
  		}
  		return false;
    }

    // returns the Manhattan distance between a and b
    private int Distance(Vector3Int a, Vector3Int b){
    	return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    // returns the earliest placed cell with min cost
    private Vector3Int GetMinCell(SortedDictionary<int, Stack<Vector3Int>> toVisit){
    
    	IEnumerator<int> keys = toVisit.Keys.GetEnumerator();
    	keys.MoveNext();
    	int minKey = keys.Current;

    	Stack<Vector3Int> minStack = toVisit[minKey];
    	Vector3Int res = minStack.Pop();
    	if(minStack.Count == 0){
    		toVisit.Remove(minKey);
    	}

    	return res;
    }


    /*
     *  Pathfollowing Functions
     */

    public void GoTo(Vector3 position){
    	if(pathRoutine != null){
    		StopMoving();
    	}
    	path = ShortestPath(NearestCell(transform.position),
    	  NearestCell(position));
    	StartMoving();
    }

    private void StartMoving(){
    	if(path.Count > 1){
    		pathRoutine = StartCoroutine(FollowPath());
    	}
    	// already at the destination
    	else{
    		onArrival.Invoke();
    		path = null;
    	}
    }

    public void StopMoving(){
    	if(pathRoutine != null){
    		StopCoroutine(pathRoutine);
    		pathRoutine = null;
    		path = null;
    	}
    }

    private IEnumerator FollowPath(){
    	float totalTime = .5f; // start a little ahead
    	int currentIdx = 0;
    	Vector3Int end = path[path.Count - 1];
    	Vector3Int nearestCell = NearestCell(transform.position);

    	float actualDt = Time.fixedDeltaTime;
    	while(nearestCell != end){

    		float dt = actualDt * speed / map.layoutGrid.cellSize.x;
    		totalTime += dt;
    		currentIdx = Mathf.Min((int)Mathf.Floor(totalTime), path.Count - 2);

    		// determine point to go towards
    		Vector3 targetPoint = Vector3.Lerp(
    			map.GetCellCenterWorld(path[currentIdx]),
    			map.GetCellCenterWorld(path[currentIdx + 1]),
    			totalTime - currentIdx);

    		DrawCellCrosshair(NearestCell(targetPoint), Color.green);

    		// compute velocity vector to move
    		Vector3 targetDir = targetPoint - transform.position;
    		Vector3 velocityDir = targetDir;
    		float maxSpeed = velocityDir.magnitude / actualDt;
    		targetDir.Normalize();

    		// don't walk over obstacles if near center of nearest cell
    		Vector3Int dx = new Vector3Int((int)Mathf.Sign(velocityDir.x), 0, 0);
    		Vector3Int dy = new Vector3Int(0, (int)Mathf.Sign(velocityDir.y), 0);
    		Vector3 toCellCenter = transform.position - map.GetCellCenterWorld(nearestCell);
    		if(map.GetTile(nearestCell + dx) != null && IsShorter(toCellCenter, map.layoutGrid.cellSize.x / 4)){
    			velocityDir.x = 0;
    		}
    		if(map.GetTile(nearestCell + dy) != null && IsShorter(toCellCenter, map.layoutGrid.cellSize.y / 4)){
    			velocityDir.y = 0;
    		}
    		velocityDir.Normalize();

    		// rewind target progress if not moving directly towards it
    		totalTime -= (1 - Mathf.Max(Vector3.Dot(targetDir, velocityDir), 0)) * dt;

    		// apply velocity
    		transform.Translate(velocityDir * Mathf.Min(speed, maxSpeed) * actualDt);

    		Quaternion rot = visionCone.transform.rotation;
    		rot.SetLookRotation(Vector3.forward, velocityDir);
    		visionCone.transform.rotation = Quaternion.Lerp(visionCone.transform.rotation, rot, turnSpeed);

    		// prepare for next loop
				nearestCell = NearestCell(transform.position);
    		yield return new WaitForSeconds(actualDt);
    	}

    	pathRoutine = null;
    	onArrival.Invoke();
    	yield break;
    }

    private bool IsShorter(Vector3 vector, float distance){
    	return vector.sqrMagnitude < Mathf.Pow(distance, 2);
    }
  }
}
