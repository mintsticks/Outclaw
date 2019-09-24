#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Outclaw.Heist {
  public class ChaseMovement : MonoBehaviour {

    [System.Serializable]
    public class LostEvent : UnityEvent<Vector3> {}

    [Header("Chasing")]
    [Inject(Id = "Active Speed")] private float speed;
    public GameObject target;
    [Inject(Id = "Raycast Layers")] private LayerMask hitLayers;
    private Coroutine chaseRoutine;
    public LostEvent onTargetLost = new LostEvent();
    private Vector3 lastSeen;

    [Header("Capture")]
    [SerializeField] private float captureRadius = .1f;
    public UnityEvent onCapture = new UnityEvent();

    public void StartChase(GameObject target) {
      if(chaseRoutine == null) {
        this.target = target;
        chaseRoutine = StartCoroutine(Chase());
      }
    }

    public void EndChase() {
      if(chaseRoutine != null) {
        StopCoroutine(chaseRoutine);
        chaseRoutine = null;
        target = null;
      }
    }

    private IEnumerator Chase() {
      while (true) {
        Vector3 toTarget = target.transform.position - transform.position;
        if(toTarget.magnitude < captureRadius) {
          chaseRoutine = null;
          target = null;
          onCapture.Invoke();
          yield break;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position,
          toTarget, Mathf.Infinity, hitLayers);
        Debug.DrawRay(transform.position, toTarget);

        if(hit.collider != null && hit.collider.gameObject.CompareTag("Player")) {
          transform.Translate(Vector3.Normalize(toTarget) * speed * Time.deltaTime);
          lastSeen = target.transform.position;
        } else {
          chaseRoutine = null;
          target = null;
          onTargetLost.Invoke(lastSeen);
          yield break;
        }

        yield return null;
      }
    }
  }
}