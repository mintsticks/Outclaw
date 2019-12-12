using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Outclaw.City{
  public class MoveAndLoop : MonoBehaviour
  {
    private float minX;
    private float maxX;
    private float xVelocity;

    [SerializeField] private Transform visuals;
    [SerializeField] private Animator anim;
    [SerializeField] private SortingGroup group;

    private PeopleSpawner spawner;

    public void Init(PeopleSpawner spawner, float speed, float animMult, int layerID){
      this.spawner = spawner;
      xVelocity = speed;

      visuals.localScale = new Vector3(
          visuals.localScale.x * Mathf.Sign(speed),
          visuals.localScale.y,
          visuals.localScale.z
        );

      anim?.SetFloat("animSpeed", animMult);

      group.sortingLayerID = layerID;
    }

    void FixedUpdate(){
      Vector3 newPos = transform.position;
      newPos.x += xVelocity * Time.fixedDeltaTime;
      transform.position = newPos;

      if(!spawner.IsInBounds(newPos.x)){
        spawner.SpawnOutsideCamera();
        Destroy(gameObject);
        return;
      }
    }
  }
}
