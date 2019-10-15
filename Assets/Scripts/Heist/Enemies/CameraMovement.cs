using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw.Heist{
  public class CameraMovement : MonoBehaviour
  {
    [SerializeField] [Range(0, 360)] private float startAngle;
    [SerializeField] [Range(0, 360)] private float endAngle;
    [SerializeField] private float turnTime;
    [SerializeField] private float pauseTime;

    [Header("Component Links")]
    [SerializeField] private GuardMovement movement;

    void Start(){
      StartCoroutine(Camera());
    }

    private IEnumerator Camera(){
      Quaternion start = Quaternion.AngleAxis(startAngle, Vector3.forward);
      Quaternion end = Quaternion.AngleAxis(endAngle, Vector3.forward);

      transform.rotation = start;
      while(true){
        yield return movement.TurnHead(end, turnTime);
        yield return new WaitForSeconds(pauseTime);
        yield return movement.TurnHead(start, turnTime);
        yield return new WaitForSeconds(pauseTime);
      }
    }
  } 
}