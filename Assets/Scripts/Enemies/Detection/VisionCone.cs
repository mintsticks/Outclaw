#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
using Outclaw.ManagedRoutine;

namespace Outclaw.Heist {
  [System.Serializable]
  public class OnDetect : UnityEvent { }
  public class VisionCone : MonoBehaviour {
    [Header("Cone Generation")]
    [Tooltip("Degrees")]
    [SerializeField] [Range(0, 360)] private float coneAngle = 90;
    [SerializeField] private float visionDistance = 1;
    [Tooltip("Extra raycasts are +- this angle in radians in to give more accuracy on corners.")]
    [SerializeField] [Range(0, 1)] private float accuracyRayOffset = .00001f;
    [Tooltip("Angle in degrees of how far appart points on the edge can be.")]
    [SerializeField] [Range(0, 360)] private float minArcOffset = 5;

    [Header("Detection")] 
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private LayerMask targetLayer;
    public OnDetect onDetect = new OnDetect();

    [Header("Component Links")] 
    [SerializeField] private MeshFilter filter;

    [SerializeField] private MeshRenderer rend;
    [Inject] private IHideablePlayer player;
    [Inject] private IPauseGame pause;

    private float AccuracyRayOffsetDeg{
      get{
        return Mathf.Rad2Deg * accuracyRayOffset;
      }
    }

    void LateUpdate() {
      if (pause.IsPaused) {
        return;
      }

      GameObject player = TestConeAccurate();
      if (player != null) {
        onDetect.Invoke();
      }
    }

    private void CreateMesh(List<Vector3> meshVerts) {
      Color[] colors = {
        Color.red,
        Color.yellow,
        Color.green,
        Color.blue,
        Color.magenta
      };
      for(int i = 0; i < meshVerts.Count; ++i){

        transform.TransformPoint(meshVerts[i]).DrawCrosshair(
          colors[i % colors.Length]);
      }

      Mesh m = filter.mesh;
      m.Clear();
      m.vertices = meshVerts.ToArray();
      List<int> tris = new List<int>();
      for (int i = 2; i < m.vertices.Length; ++i) {
        tris.Add(0);
        tris.Add(i);
        tris.Add(i - 1);
      }

      m.triangles = tris.ToArray();
    }

    public void SetVisible(bool visible) {
      rend.enabled = visible;
    }

    private GameObject TestConeAccurate() {
      GetVerticesInRange(out List<Vector3> points, out Dictionary<Vector3, float> angleCache);
      points.Sort((Vector3 a, Vector3 b) => {
          return (int)Mathf.Sign(angleCache[a] - angleCache[b]);
        });


      GameObject target = TestPoints(points, angleCache, 
        out List<Vector3> meshBoarder);
      CreateMesh(meshBoarder);

      return target;
    }

    private void GetVerticesInRange(out List<Vector3> points, 
          out Dictionary<Vector3, float> angleCache){
      Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position,
        visionDistance, hitLayers);

      points = new List<Vector3>();
      angleCache = new Dictionary<Vector3, float>();
      foreach(Collider2D col in cols){
        GetVerticesInRangeOnCollider(col, points, angleCache);
      }
    }

    // fills points with points in range on the collider,
    // angleCache will have the angle from transform.up to the direction to the point
    private void GetVerticesInRangeOnCollider(Collider2D col, 
          List<Vector3> points, Dictionary<Vector3, float> angleCache){
      if(col is CompositeCollider2D){
        GetVerticesInRangeOnCompositeCollider((CompositeCollider2D)col,
          points, angleCache);
      }
      else if(col is BoxCollider2D){
        GetVerticesInRangeOnBoxCollider((BoxCollider2D)col,
          points, angleCache);
      }

      //TODO: add vertex detection for other colliders
    }

    // fills points with points in range on the collider,
    // angleCache will have the angle from transform.up to the direction to the point
    private void GetVerticesInRangeOnBoxCollider(BoxCollider2D col, 
          List<Vector3> points, Dictionary<Vector3, float> angleCache){
      Vector3[] corners = new Vector3[]{
        col.bounds.max,
        col.bounds.min,
        new Vector3(col.bounds.min.x, col.bounds.max.y, 0),
        new Vector3(col.bounds.max.x, col.bounds.min.y, 0)
      };

      foreach(Vector3 corner in corners){
        // out of radius
        if((corner - transform.position).sqrMagnitude > visionDistance * visionDistance){
          continue;
        }

        float angle = AngleTo(corner);

        // already have point
        if(angleCache.ContainsValue(angle)){
          continue;
        }

        // out of angle
        if(Mathf.Abs(angle) > coneAngle / 2){
          continue;
        }

        points.Add(corner);
        angleCache.Add(corner, angle);
      }
    }

    // fills points with points in range on the collider,
    // angleCache will have the angle from transform.up to the direction to the point
    private void GetVerticesInRangeOnCompositeCollider(CompositeCollider2D col, 
          List<Vector3> points, Dictionary<Vector3, float> angleCache){
      int numPaths = col.pathCount;
      for(int i = 0; i < numPaths; ++i){

        List<Vector2> verts = new List<Vector2>();
        col.GetPath(i, verts);
        foreach(Vector2 vert in verts){
          // out of radius
          if((vert - (Vector2)transform.position).sqrMagnitude > visionDistance * visionDistance){
            continue;
          }

          float angle = AngleTo(vert);

          // already have point
          if(angleCache.ContainsValue(angle)){
            continue;
          }

          // out of angle
          if(Mathf.Abs(angle) > coneAngle / 2){
            continue;
          }

          points.Add(vert);
          angleCache.Add(vert, angle);
        }
      }
    }

    // returns angle of vector from current position to pt
    private float AngleTo(Vector3 pt){
      Vector3 dir = pt - transform.position;
      float angle = Vector2.SignedAngle(transform.up, dir);
      return angle;
    }

    private GameObject TestPoints(List<Vector3> points, Dictionary<Vector3, float> angleCache,
        out List<Vector3> meshBoarder){
      meshBoarder = new List<Vector3>();
      meshBoarder.Add(Vector3.zero);

      float minAngle = -coneAngle / 2;
      float maxAngle = -minAngle;
      GetExtraRotation(out Quaternion negRot, out Quaternion posRot);

      // jump to first point in angle range
      int startIdx = FirstAngleInRange(points, angleCache, minAngle);

      // test smallest angle
      Quaternion minRot = Quaternion.AngleAxis(minAngle, Vector3.forward);
      Vector3 hitPt = TestRay(minRot * transform.up, out GameObject objHit);
      meshBoarder.Add(hitPt);

      hitPt = TestRay(minRot * posRot * transform.up, out objHit);
      GameObject target = FoundTarget(objHit) ? objHit : null;
      meshBoarder.Add(hitPt);
      target = target ?? (FoundTarget(objHit) ? objHit : null);
      bool prevCastMissed = objHit == null;

      Vector3 preHit, mainHit, postHit;

      // test point directly and some offset around the point
      int i = startIdx;
      for(; i < points.Count && angleCache[points[i]] < maxAngle; ++i){

        Vector3 dir = (points[i] - transform.position).normalized;

        // need to smooth vert across arc if previous and current both missed
        preHit = TestRay(negRot * dir, out objHit);
        CheckRangeBetween(meshBoarder, 
          (i == startIdx) ? minAngle : angleCache[points[i - 1]], 
          prevCastMissed,
          angleCache[points[i]], 
          objHit == null);
        target = target ?? (FoundTarget(objHit) ? objHit : null);
        
        mainHit = TestRay(dir, out objHit);
        target = target ?? (FoundTarget(objHit) ? objHit : null);

        postHit = TestRay(posRot * dir, out objHit);
        target = target ?? (FoundTarget(objHit) ? objHit : null);

        if(!IsColinear(preHit, mainHit, postHit)){
          meshBoarder.Add(preHit);
          meshBoarder.Add(mainHit);
          meshBoarder.Add(postHit);
        }
        else{
          meshBoarder.Add(mainHit);
        }

        prevCastMissed = objHit == null;
      }

      // test largest edge
      Quaternion maxRot = Quaternion.AngleAxis(maxAngle, Vector3.forward);
      hitPt = TestRay(maxRot * negRot * transform.up, out objHit);
      CheckRangeBetween(meshBoarder,
        (i == startIdx) ? minAngle : angleCache[points[i - 1]],
        prevCastMissed,
        maxAngle,
        objHit == null);
      meshBoarder.Add(hitPt);
      target = target ?? (FoundTarget(objHit) ? objHit : null);
      hitPt = TestRay(maxRot * transform.up, out objHit);
      meshBoarder.Add(hitPt);
      target = target ?? (FoundTarget(objHit) ? objHit : null);

      return target;
    }

    private void CheckRangeBetween(List<Vector3> meshBoarder, float prevAngle, 
        bool prevCastMissed, float curAngle, bool curCastMissed){

      if(curCastMissed && prevCastMissed){
        AddVertsForArc(meshBoarder, prevAngle, curAngle);
      }
      else if(!curCastMissed && prevCastMissed){
        AddMissHitIntersection(meshBoarder, curAngle, prevAngle);
      }
      else if(curCastMissed && !prevCastMissed){
        AddMissHitIntersection(meshBoarder, prevAngle, curAngle);
      }
      else{
        MaybeAddHitHitIntersection(meshBoarder, prevAngle, curAngle);
      }
    }

    // adds verts between startAngle and endAngle to approximate a circle arc
    private void AddVertsForArc(List<Vector3> meshBoarder, float startAngle, float endAngle){
      Vector3 toEdge = transform.up * visionDistance;
      for(float angle = startAngle + minArcOffset; angle < endAngle; angle += minArcOffset){
        Vector3 newPt = Quaternion.AngleAxis(angle, Vector3.forward) * toEdge
          + transform.position;
        meshBoarder.Add(transform.InverseTransformPoint(newPt));
      }
    }

    private void MaybeAddHitHitIntersection(List<Vector3> meshBoarder,
        float prevAngle, float curAngle){

      // shift slightly towards prevAngle in case vertex is concave corner
      curAngle -= AccuracyRayOffsetDeg;
      prevAngle += AccuracyRayOffsetDeg;
      RaycastHit2D hit = Physics2D.Raycast(transform.position,
        Quaternion.AngleAxis(curAngle, Vector3.forward) * transform.up,
        visionDistance, hitLayers & (~targetLayer));
      Vector3 maxHitPt = hit.point;
      Vector3 maxHitNorm = hit.normal;

      // 3 points are in line, mesh created is fine
      if(IsColinear(meshBoarder[meshBoarder.Count - 2], meshBoarder[meshBoarder.Count - 1], 
            transform.InverseTransformPoint(maxHitPt))){
        return;
      }

      float minRange = prevAngle;
      float maxRange = curAngle;

      // check for intersect near prevAngle
      hit = Physics2D.Raycast(transform.position,
        Quaternion.AngleAxis(prevAngle, Vector3.forward) * transform.up,
        visionDistance, hitLayers & (~targetLayer));
      Vector3 hitPt = hit.point;
      Vector3 surfaceDir = Vector3.Cross(hit.normal, Vector3.forward);
      Vector3 rayDir = hitPt - transform.position;
      FindCircleLineIntersection(surfaceDir, rayDir,
        out float t0, out float t1);
      Vector3 intersectionPoint;
      float intersectionAngle;
      bool hasPrevIntersect = HasIntersectionInAngleRange(
        hitPt + (surfaceDir * t0), hitPt + (surfaceDir * t1), prevAngle, 
        curAngle, out intersectionPoint, out intersectionAngle);
      if(hasPrevIntersect){
        minRange = intersectionAngle;
        meshBoarder.Add(transform.InverseTransformPoint(intersectionPoint));
      }

      // check for intersect near curAngle
      surfaceDir = Vector3.Cross(maxHitNorm, Vector3.forward);
      rayDir = maxHitPt - transform.position;
      FindCircleLineIntersection(surfaceDir, rayDir, out t0, out t1);
      bool hasCurIntersect = HasIntersectionInAngleRange(
        maxHitPt + (surfaceDir * t0), maxHitPt + (surfaceDir * t1), prevAngle, 
        curAngle, out intersectionPoint, out intersectionAngle);
      if(hasCurIntersect){
        maxRange = intersectionAngle;
        if(hasPrevIntersect){
          AddVertsForArc(meshBoarder, minRange, maxRange);
        }
        meshBoarder.Add(transform.InverseTransformPoint(intersectionPoint));
      }
    }

    // find area of triangle from 3 points, colinear if near 0
    private bool IsColinear(Vector3 a, Vector3 b, Vector3 c){
      Vector3 v = (b - a).normalized;
      Vector3 u = (c - a).normalized;
      Vector3 cross = Vector3.Cross(v, u);

      return cross.magnitude < .01f;
    }

    /*
     * Since there's a hit on one side and a miss on the other, there's probably
     * an intersection of some ray and the ground that's missing.
     * 
     * Assumes surface between hit and missing part is flat
     */
    private void AddMissHitIntersection(List<Vector3> meshBoarder, float hitAngle,
          float missAngle){

      // shift slightly towards missAngle in case vertex is concave corner
      hitAngle += Mathf.Sign(missAngle - hitAngle) * AccuracyRayOffsetDeg;
      RaycastHit2D hit = Physics2D.Raycast(transform.position,
        Quaternion.AngleAxis(hitAngle, Vector3.forward) * transform.up,
        visionDistance, hitLayers & (~targetLayer));
      Vector3 hitPt = hit.point;

      Vector3 surfaceDir = Vector3.Cross(hit.normal, Vector3.forward);
      Vector3 rayDir = hitPt - transform.position;

      FindCircleLineIntersection(surfaceDir, rayDir,
        out float t0, out float t1);

      // figure out if there's a point between the angles
      float minAngle = Mathf.Min(hitAngle, missAngle);
      float maxAngle = Mathf.Max(hitAngle, missAngle);
      if(!HasIntersectionInAngleRange(hitPt + (surfaceDir * t0), 
          hitPt + (surfaceDir * t1), minAngle, maxAngle,
          out Vector3 intersectionPoint, out float intersectionAngle)){
        return;
      }

      // build the mesh
      if(minAngle == missAngle){
        AddVertsForArc(meshBoarder, minAngle, intersectionAngle);
      }
      meshBoarder.Add(transform.InverseTransformPoint(intersectionPoint));
      if(maxAngle == missAngle){
        AddVertsForArc(meshBoarder, intersectionAngle, maxAngle);
      }
    }

    /*
     * Finds circle-line intersection based on:
     *   surfaceDir: the vector that points along the line
     *   rayDir: the vector from the circle center to a point on the line
     *   visionDistance: the radius of the circle
     */
    private void FindCircleLineIntersection(Vector3 surfaceDir, Vector3 rayDir,
        out float t0, out float t1){

      float dot = Vector3.Dot(surfaceDir, rayDir);
      float surfaceDot = Vector3.Dot(surfaceDir, surfaceDir);
      float rayDot = Vector3.Dot(rayDir, rayDir);

      // compute circle, surface line intersections as t0 and t1
      float firstTerm = -dot;
      float rootTerm = Mathf.Sqrt((dot * dot) - (surfaceDot * rayDot)
        + (surfaceDot * visionDistance * visionDistance));
      float denomTerm = surfaceDot;

      t0 = (firstTerm + rootTerm) / denomTerm;
      t1 = (firstTerm - rootTerm) / denomTerm;
    }

    // returns true if either t0 or t1 * rayDir + origin results in
    //   an angle from transform.up within minAngle and maxAngle
    // if true, the results are put into angle and point
    private bool HasIntersectionInAngleRange(Vector3 p0, Vector3 p1, 
        float minAngle, float maxAngle, out Vector3 point, out float angle){

      point = p0;
      angle = AngleTo(point);

      if(!(minAngle < angle && angle < maxAngle)){
        point = p1;
        angle = AngleTo(point);
        
        // neither intersection is in the angle range, return false
        if(!(minAngle < angle && angle < maxAngle)){
          return false;
        }
      }

      return true;
    }

    private bool FoundTarget(GameObject hit){
      return !player.Hidden && hit == player.PlayerTransform.gameObject;
    }

    private int FirstAngleInRange(List<Vector3> points, Dictionary<Vector3, float> angleCache,
        float angle){
      int startIdx = 0;
      while(startIdx < points.Count && angleCache[points[startIdx]] < angle){
        ++startIdx;
      }
      return startIdx;
    }

    private void GetExtraRotation(out Quaternion negRot, out Quaternion posRot){
      float degrees = AccuracyRayOffsetDeg;
      negRot = Quaternion.AngleAxis(-degrees, Vector3.forward).normalized;
      posRot = Quaternion.AngleAxis(degrees, Vector3.forward).normalized;
    }

    // returns the point in local space hit by a raycast going along dir
    // if the target is found, target is set to the target
    private Vector3 TestRay(Vector3 dir, out GameObject objectHit){
      RaycastHit2D hit = Physics2D.Raycast(transform.position,
        dir, visionDistance, hitLayers);


      objectHit = hit.collider?.gameObject;

      // found the target
      if (hit.collider != null && (1 << hit.collider.gameObject.layer & targetLayer) != 0) {

        // recast while ignoring target
        hit = Physics2D.Raycast(transform.position,
          dir, visionDistance, hitLayers & (~targetLayer));
      }

      Vector3 worldLineEnd;
      if (hit.collider == null) {
        // no hit, draw end of vision
        worldLineEnd = (dir * visionDistance) + transform.position;
      } else {
        // hit, get position in local space
        worldLineEnd = hit.point;
      }

      return transform.InverseTransformPoint(worldLineEnd);
    }

    private void DrawCrosshair(Vector3 point){
      point.DrawCrosshair(Color.white);
    }

    private void DrawHitOrMissRay(Vector3 dest, bool hit){
      Debug.DrawLine(transform.position, dest,
        hit ? Color.blue : Color.red);
    }
  }
}