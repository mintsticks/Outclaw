#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
using Outclaw.ManagedRoutine;

namespace Outclaw.Heist{
  public class GuardAim : MonoBehaviour
  {
    [System.Serializable]
    public class OnFire : UnityEvent<Vector3> {}

    [Header("Shooting")]
    [Tooltip("Degrees per second")]
    [SerializeField] private float maxLaserTurnSpeed = 5f;
    [SerializeField] private float maxShootDist = 10f;
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float postFirePause = .1f;

    [SerializeField] private OnFire onFire;
    [SerializeField] private UnityEvent onAimEnd;

    [Header("Component Links")]
    [SerializeField] private LineRenderer laserLine;
    [SerializeField] private GuardMovement movement;
    [Inject] private IHideablePlayer player;

    private ManagedCoroutine<GameObject> aimRoutine;

    void Start(){
      aimRoutine = new ManagedCoroutine<GameObject>(this, Aim);
    }

    public void StartAim(GameObject target){
      laserLine.enabled = true;
      aimRoutine.StartCoroutine(target);
    }

    public void StopAim(){
      laserLine.enabled = false;
      aimRoutine.StopCoroutine();
    }

    private IEnumerator Aim(GameObject target){
      Vector3 targetPos = target.transform.position;
      Quaternion dest = Quaternion.LookRotation(Vector3.forward,
        targetPos - transform.position);
      Quaternion curDir = movement.VisionRotation;

      while(curDir != dest){
        curDir = Quaternion.RotateTowards(curDir, dest, maxLaserTurnSpeed 
          * Time.deltaTime);
        Vector3 rayDir = curDir * Vector3.up;

        // see if player was hit
        RaycastHit2D hit = Physics2D.Raycast(transform.position,
          rayDir, maxShootDist, hitLayers);
        if(hit.collider != null && !player.Hidden && (1 << hit.collider.gameObject.layer & playerLayer) != 0){
          laserLine.enabled = false;
          onFire.Invoke(hit.point);

          yield return new WaitForSeconds(postFirePause);
          break;
        }

        // player not hit, redraw the laser line
        Vector3 end = Vector3.zero;
        if(hit.collider != null){
          end = hit.point;
        }
        else{
          end = rayDir.normalized * maxShootDist + transform.position;
        }
        UpdateLaser(end);
        yield return null;
      }

      laserLine.enabled = false;
      onAimEnd.Invoke();
      aimRoutine.Clear();
      yield break;
    }

    private void UpdateLaser(Vector3 end){
      laserLine.SetPositions(new Vector3[] {transform.position, end});
    }
  }
}
