using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
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
	    [SerializeField] private Tilemap map = null;
	    private Grid grid = null;

	    [SerializeField] private Transform destination = null;

	    public float speed = 5;
	    public float minTargetDistance = .8f;
	    private List<Vector3Int> path = null;
	    private Coroutine pathRoutine = null;

	    void Start(){
	    	grid = map.layoutGrid;
	    	ComputePath();
	    }

	    void Update(){
	    	DrawCellCrosshair(NearestCell(transform.position), Color.white);
	    	DrawCellCrosshair(NearestCell(destination.position), Color.black);

	    	if(path != null){
		    	foreach(Vector3Int cell in path){
		    		DrawCellCrosshair(cell, Color.blue);
		    	}
	    	}
	    }

	    public void ComputePath(){

	    	path = ShortestPath(
	    			NearestCell(transform.position),
	    			NearestCell(destination.position)
	    		);
	    }

	    // returns the cell closest to a given point
	    private Vector3Int NearestCell(Vector3 point){
	    	return new Vector3Int(
	    			(int)Mathf.Floor(point.x / grid.cellSize.x),
	    			(int)Mathf.Floor(point.y / grid.cellSize.y),
	    			0
	    		);
	    }

	    private void DrawCellCrosshair(Vector3Int cellCoord, Color color){
	    	Debug.DrawRay(map.GetCellCenterWorld(cellCoord), Vector3.up / 2, color);
	    	Debug.DrawRay(map.GetCellCenterWorld(cellCoord), Vector3.right / 2, color);

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
	    			if(!visited.Contains(neighbor) && map.GetTile(neighbor) == null){
	    				int cost = cellCurrentBest[current].Item1 + Distance(current, neighbor);

	    				if(!cellCurrentBest.ContainsKey(neighbor) || cellCurrentBest[neighbor].Item1 > cost){
	    					cellCurrentBest[neighbor] = new Tuple<int, Vector3Int>(cost, current);

	    					// prioritize by min cost to travel + min distance to go
	    					int weightedCost = cost + Distance(neighbor, end);
	    					if(!toVisit.ContainsKey(weightedCost)){
	    						toVisit[weightedCost] = new Stack<Vector3Int>();
	    					}
	    					toVisit[weightedCost].Push(neighbor);
	    				}
	    			}
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

	    public void Go(){
	    	if(path != null){
		    	if(pathRoutine != null){
		    		StopMoving();
		    	}
		    	StartMoving();
	    	}
	    }

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
	    }

	    public void StopMoving(){
	    	if(pathRoutine != null){
	    		StopCoroutine(pathRoutine);
	    		pathRoutine = null;
	    	}
	    }

	    private IEnumerator FollowPath(){
	    	float totalTime = 0;
	    	int currentIdx = 0;
	    	Vector3Int end = path[path.Count - 1];
	    	Vector3Int nearestCell = NearestCell(transform.position);

	    	float fixedDt = Time.fixedDeltaTime;
	    	while(nearestCell != end){
	    		float dt = fixedDt * speed;
	    		totalTime += dt;
	    		currentIdx = Mathf.Min((int)Mathf.Floor(totalTime), path.Count - 2);

	    		// determine point to go towards
	    		Vector3 targetPoint = Vector3.Lerp(
	    			map.GetCellCenterWorld(path[currentIdx]),
	    			map.GetCellCenterWorld(path[currentIdx + 1]),
	    			totalTime - currentIdx);

	    		DrawCellCrosshair(NearestCell(targetPoint), Color.green);

	    		// compute velocity vector to move
	    		Vector3 velocityDir = targetPoint - transform.position;
	    		float maxSpeed = velocityDir.magnitude;

	    		// don't walk over obstacles
	    		Vector3Int dx = new Vector3Int((int)Mathf.Sign(velocityDir.x), 0, 0);
	    		Vector3Int dy = new Vector3Int(0, (int)Mathf.Sign(velocityDir.y), 0);
	    		if(map.GetTile(nearestCell + dx) != null){
	    			velocityDir.x = 0;
	    		}
	    		if(map.GetTile(nearestCell + dy) != null){
	    			velocityDir.y = 0;
	    		}

	    		// apply velocity
	    		velocityDir.Normalize();
	    		GetComponent<Rigidbody2D>().velocity = velocityDir * Mathf.Min(speed, maxSpeed);
	    		Debug.DrawRay(transform.position, velocityDir, Color.red);

	    		// prepare for next loop
				nearestCell = NearestCell(transform.position);
	    		yield return new WaitForSeconds(fixedDt);
	    	}

	    	GetComponent<Rigidbody2D>().velocity = Vector2.zero;
	    	yield break;
	    }
	}
}
