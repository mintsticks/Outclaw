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
    [SerializeField] private int numSamples = 10;
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
      List<Vector3> points = GetVerticesInRange();
      Dictionary<Vector3, float> angleCache = new Dictionary<Vector3, float>();

      foreach(Vector3 point in points){
        angleCache.Add(point, AngleTo(point));
      }
      points.Sort((Vector3 a, Vector3 b) => {
          return (int)Mathf.Sign(angleCache[a] - angleCache[b]);
        });


      GameObject target = TestPoints(points, angleCache, 
        out List<Vector3> meshBoarder);
      CreateMesh(meshBoarder);

      return target;
    }

    private List<Vector3> GetVerticesInRange(){
      Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position,
        visionDistance, hitLayers);

      List<Vector3> res = new List<Vector3>();
      foreach(Collider2D col in cols){
        res.AddRange(GetVerticesInRangeOnCollider(col));
      }
      return res;
    }

    private List<Vector3> GetVerticesInRangeOnCollider(Collider2D col){
      if(col is CompositeCollider2D){
        return GetVerticesInRangeOnCompositeCollider((CompositeCollider2D)col);
      }
      else if(col is BoxCollider2D){
        return GetVerticesInRangeOnBoxCollider((BoxCollider2D)col);
      }

      // case not handled
      return new List<Vector3>();
    }

    private List<Vector3> GetVerticesInRangeOnBoxCollider(BoxCollider2D col){
      List<Vector3> res = new List<Vector3>();
      Vector3 topRight = col.bounds.max;
      Vector3 bottomLeft = col.bounds.min;
      Vector3 topLeft = new Vector3(bottomLeft.x, topRight.y, 0);
      Vector3 bottomRight = new Vector3(topRight.x, bottomLeft.y, 0);

      if((topRight - transform.position).sqrMagnitude < visionDistance * visionDistance){
        res.Add(topRight);
      }
      if((bottomLeft - transform.position).sqrMagnitude < visionDistance * visionDistance){
        res.Add(bottomLeft);
      }
      if((topLeft - transform.position).sqrMagnitude < visionDistance * visionDistance){
        res.Add(topLeft);
      }
      if((bottomRight - transform.position).sqrMagnitude < visionDistance * visionDistance){
        res.Add(bottomRight);
      }

      return res;
    }

    private List<Vector3> GetVerticesInRangeOnCompositeCollider(CompositeCollider2D col){
      List<Vector3> res = new List<Vector3>();
      int numPaths = col.pathCount;
      for(int i = 0; i < numPaths; ++i){

        List<Vector2> points = new List<Vector2>();
        col.GetPath(i, points);
        foreach(Vector2 pt in points){
          if((pt - (Vector2)transform.position).sqrMagnitude < visionDistance * visionDistance){
            res.Add(pt);
          }
        }
      }
      return res;
    }

    // returns angle of vector from current position to pt
    private float AngleTo(Vector3 pt){
      Vector3 dir = pt - transform.position;

      // using negative z axis because this function is in right handed system(?)
      float angle = Vector3.SignedAngle(dir, transform.up, -Vector3.forward);
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
      meshBoarder.Add(TestRay(minRot * transform.up, out GameObject objHit));
      GameObject target = FoundTarget(objHit) ? objHit : null;
      meshBoarder.Add(TestRay(minRot * posRot * transform.up, out objHit));
      target = target ?? (FoundTarget(objHit) ? objHit : null);
      bool prevCastMissed = objHit == null;

      // test point directly and some offset around the point
      Vector3 hitPt;
      int i = startIdx;
      for(; i < points.Count && angleCache[points[i]] < maxAngle; ++i){

        Vector3 dir = (points[i] - transform.position).normalized;

        // need to smooth vert across arc if previous and current both missed
        hitPt = TestRay(negRot * dir, out objHit);
        if(objHit == null && prevCastMissed){
          AddVertsToSmooth(meshBoarder, 
            (i == startIdx) ? minAngle : angleCache[points[i - 1]],
            angleCache[points[i]]);
        }
        meshBoarder.Add(hitPt);
        target = target ?? (FoundTarget(objHit) ? objHit : null);

        meshBoarder.Add(TestRay(dir, out objHit));
        target = target ?? (FoundTarget(objHit) ? objHit : null);
        meshBoarder.Add(TestRay(posRot * dir, out objHit));
        target = target ?? (FoundTarget(objHit) ? objHit : null);

        prevCastMissed = objHit == null;
      }

      // test largest edge
      Quaternion maxRot = Quaternion.AngleAxis(maxAngle, Vector3.forward);
      hitPt = TestRay(maxRot * negRot * transform.up, out objHit);
      if(objHit == null && prevCastMissed){
        AddVertsToSmooth(meshBoarder, 
          (i == startIdx) ? minAngle : angleCache[points[i - 1]],
          maxAngle);
      }
      meshBoarder.Add(hitPt);
      target = target ?? (FoundTarget(objHit) ? objHit : null);
      meshBoarder.Add(TestRay(maxRot * transform.up, out objHit));
      target = target ?? (FoundTarget(objHit) ? objHit : null);

      return target;
    }

    private void AddVertsToSmooth(List<Vector3> meshBoarder, float startAngle, float endAngle){
      Vector3 toEdge = transform.up * visionDistance;
      for(float angle = startAngle + minArcOffset; angle < endAngle; angle += minArcOffset){
        Vector3 newPt = Quaternion.AngleAxis(angle, Vector3.forward) * toEdge
          + transform.position;
        meshBoarder.Add(transform.InverseTransformPoint(newPt));
      }
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
      float degrees = Mathf.Rad2Deg * accuracyRayOffset;
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
      Debug.DrawLine(point + (Vector3.up / 3), point + (Vector3.down / 3), Color.white);
      Debug.DrawLine(point + (Vector3.left / 3), point + (Vector3.right / 3), Color.white);
    }
  }
}