using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

namespace Outclaw.Heist{
  [RequireComponent(typeof(SpriteMask))]
  [RequireComponent(typeof(SortingGroup))]
  public class PatternOnSenseElement : MonoBehaviour, ISenseElement 
  {
    [Header("Main Sprite")]
    [SerializeField] private SpriteRenderer sourceSprite;

    [Header("Pattern")]
    [SerializeField] private SpriteMask patternMask;
    [SerializeField] private Transform patternTransform;
    [SerializeField] private SpriteRenderer patternSprite;
    [SerializeField] private Color hideColor = new Color(1, 1, 1, 0);
    [SerializeField] private Color showColor = new Color(1, 1, 1, 1);

    [Inject] private ISenseVisuals senseVisuals;

    void Awake() {
      senseVisuals.RegisterSenseElement(this);

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

      
      patternSprite.color = hideColor;
    }

    public void UpdateElement(float animationProgress) {
      Color color = Color.Lerp(hideColor, showColor, animationProgress);
      patternSprite.color = color;
    }

    public void OnActivate(){}
    public void OnDeactivate(){}
  }
}
