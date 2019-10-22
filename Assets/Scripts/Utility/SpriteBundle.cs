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
    }

    void OnEnable(){
      foreach(var sprite in sprites){
        sprite.enabled = true;
      }
    }

    void OnDisable(){
      foreach(var sprite in sprites){
        sprite.enabled = false;
      }
    }
    
    [ContextMenu("SetColor")]
    private void SetColor() {
      foreach (var sprite in sprites) {
        sprite.color = spriteFilterColor;
      }
    }
    
    public void SetColor(Color color) {
      if (sprites == null) {
        return;
      }
      foreach (var sprite in sprites) {
        sprite.color = color;
      }
    }

    public Color GetColor() {
      if (sprites == null || sprites.Length <= 0) {
        return Color.white;
      }

      return sprites[0].color;
    }
  }
}
