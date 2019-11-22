using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw.City{
  public class MoveAndLoop : MonoBehaviour
  {
    private float minX;
    private float maxX;
    private float xVelocity;

    [SerializeField] private Transform visuals;

    public void Init(float min, float max, float speed){
      minX = min;
      maxX = max;
      xVelocity = speed;

      visuals.localScale = new Vector3(
          visuals.localScale.x * Mathf.Sign(speed),
          visuals.localScale.y,
          visuals.localScale.z
        );
    }

    void Update(){
      Vector3 newPos = transform.position;
      newPos.x += xVelocity * Time.deltaTime;
      if(newPos.x < minX){
        newPos.x = maxX;
      }
      else if(newPos.x > maxX){
        newPos.x = minX;
      }

      transform.position = newPos;
    }
  }
}
