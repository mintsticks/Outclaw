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

      // force mask size to be the same size as the sourceSprite size
      if(sourceSprite.transform.lossyScale != transform.lossyScale){
        transform.localScale = new Vector3(
            sourceSprite.transform.lossyScale.x / transform.lossyScale.x,
            sourceSprite.transform.lossyScale.y / transform.lossyScale.y,
            1
          );
      }

      // invert global scale so pattern is consistent size
      Vector3 origScale = patternTransform.lossyScale;
      patternTransform.localScale = new Vector3(1 / origScale.x, 1 / origScale.y, 1);

      // set the mask
      patternMask.sprite = sourceSprite.sprite;

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
