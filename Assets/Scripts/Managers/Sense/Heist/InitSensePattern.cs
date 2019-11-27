using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

namespace Outclaw.Heist{
  [RequireComponent(typeof(SpriteMask))]
  [RequireComponent(typeof(SortingGroup))]
  [RequireComponent(typeof(ChangeColorOnSenseElement))]
  public class InitSensePattern : MonoBehaviour
  {
    [Header("Main Sprite")]
    [SerializeField] private SpriteRenderer sourceSprite;

    [Header("Pattern")]
    [SerializeField] private SpriteMask patternMask;
    [SerializeField] private Transform patternTransform;
    [SerializeField] private SpriteRenderer patternSprite;

    void Awake() {
      // can't do anything without a source sprite
      if(sourceSprite == null){
        patternMask.sprite = null;
        return;
      }

      InitMaskScale();

      // set the mask
      patternMask.sprite = sourceSprite.sprite;

      InitPattern();
    }

    private void InitMaskScale(){
      Vector3 newScale = transform.localScale;

      // force mask size to be the same size as the sourceSprite size
      if(sourceSprite.transform.lossyScale != transform.lossyScale){
        newScale = new Vector3(
            sourceSprite.transform.lossyScale.x / transform.lossyScale.x,
            sourceSprite.transform.lossyScale.y / transform.lossyScale.y,
            1
          );
      }

      // source sprite is expanded beyond default sprite shape
      if(sourceSprite.drawMode != SpriteDrawMode.Simple){
        Bounds totalBound = sourceSprite.bounds;
        Bounds tileBound = sourceSprite.sprite.bounds;

        // multiply by ratio of total / orig size, but need to divide by scale 
        //   to get true total size
        newScale.x *= totalBound.size.x / sourceSprite.transform.lossyScale.x 
          / tileBound.size.x;
        newScale.y *= totalBound.size.y / sourceSprite.transform.lossyScale.y 
          / tileBound.size.y;
      }

      transform.localScale = newScale;
    }

    private void InitPattern(){
      // invert global scale so pattern is consistent size
      Vector3 origScale = patternTransform.lossyScale;
      patternTransform.localScale = new Vector3(1 / origScale.x, 1 / origScale.y, 1);

      // set up pattern
      Bounds bounds = sourceSprite.sprite.bounds;
      transform.position = sourceSprite.transform.TransformPoint(bounds.center);

      // reapply global scale to get correct total size back
      Vector2 patternSize = bounds.size;
      patternSize.x *= origScale.x;
      patternSize.y *= origScale.y;
      patternSprite.size = patternSize;

    }
  }
}
