using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw{
  public class SpriteBundle : MonoBehaviour {
    [SerializeField]
    private Color spriteFilterColor;
    
    private SpriteRenderer[] sprites;

    void Awake(){
      sprites = GetComponentsInChildren<SpriteRenderer>(true);
      foreach (var sprite in sprites) {
        sprite.color = spriteFilterColor;
      }
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
    
    [ContextMenu("SetColor")]
    private void SetColor() {
      foreach (var sprite in sprites) {
        sprite.color = spriteFilterColor;
      }
    }
  }
}
