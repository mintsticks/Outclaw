#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
using Outclaw.ManagedRoutine;

namespace Outclaw.Heist{
  public class VisionCone : MonoBehaviour
  {
    [System.Serializable]
    public class OnDetect : UnityEvent<GameObject>{}

    [Header("Detection")]
    [SerializeField] [Range(0, 360)] private float coneAngle = 90;
    [SerializeField] private float visionDistance = 1;
    [SerializeField] private int numSamples = 10;
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private LayerMask playerLayer;
    public OnDetect onDetect = new OnDetect();

    [Header("Component Links")]
    [SerializeField] private MeshFilter filter;
    [SerializeField] private MeshRenderer rend;
    [Inject] private IHideablePlayer player;
    [Inject] private IPauseGame pause;

    void Update(){
      if(!pause.IsPaused){
        GameObject player = TestCone();
        if(player != null){
          onDetect.Invoke(player);
        }
      }
    }

    // returns if the player was found
    private GameObject TestCone(){

      GameObject player = null;
      List<Vector3> meshVerts = new List<Vector3>();
      meshVerts.Add(Vector3.zero);

      Vector3 coneStart = Quaternion.AngleAxis(-coneAngle/2, Vector3.forward) 
        * transform.up;
      coneStart.Normalize();

      Quaternion rotInverse = Quaternion.Inverse(transform.rotation);

      for(int i = 0; i < numSamples; ++i){

        // cast ray into world
        Vector3 currVec = Quaternion.AngleAxis(coneAngle * i / numSamples, Vector3.forward) 
          * coneStart;
        currVec.Normalize();
        RaycastHit2D hit = Physics2D.Raycast(transform.position,
          currVec, visionDistance, hitLayers);

        // found the player
        if(hit.collider != null && (1 << hit.collider.gameObject.layer & playerLayer) != 0){
          if(!this.player.Hidden && player == null){
            player = hit.collider.gameObject;
          }

          // recast while ignoring player
          hit = Physics2D.Raycast(transform.position,
            currVec, visionDistance, hitLayers & (~playerLayer));
        }

        Vector3 worldLineEnd;
        if(hit.collider == null){ // no hit, draw end of vision
          worldLineEnd = (currVec * visionDistance) + transform.position;
        } 
        else { // hit, get position in local space
          worldLineEnd = hit.point;
        }

        meshVerts.Add(transform.InverseTransformPoint(worldLineEnd));

      }

      CreateMesh(meshVerts);

      return player;
    }

    private void CreateMesh(List<Vector3> meshVerts){
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
    }

    public void SetVisible(bool visible){
      rend.enabled = visible;
    }
  }
}