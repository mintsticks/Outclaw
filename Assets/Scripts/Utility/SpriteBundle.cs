using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw{
  public class SpriteBundle : MonoBehaviour
  {
    private SpriteRenderer[] sprites;

    void Awake(){
      sprites = GetComponentsInChildren<SpriteRenderer>(true);
    }

    void OnEnable(){
      foreach(SpriteRenderer sprite in sprites){
        sprite.enabled = true;
      }
    }

    void OnDisable(){
      foreach(SpriteRenderer sprite in sprites){
        sprite.enabled = false;
      }
    }
  }
}
