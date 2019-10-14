using System.Collections;
using City;
using Outclaw.City;
using UnityEngine;
using Zenject;

namespace Outclaw {
  public class ObjectivePointer : MonoBehaviour {
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    
    [SerializeField]
    private AnimationCurve distanceOpacityCurve =
      new AnimationCurve(new Keyframe(0, 1f), new Keyframe(10, 0f));

    [SerializeField]
    private float fadeTime;
    
    [Inject] 
    private IObjectiveManager objectiveManager;

    [Inject] 
    private IObjectiveTransformManager objectiveTransformManager;

    [Inject]
    private ISenseManager senseManager;

    [Inject]
    private IPlayer player;

    private IEnumerator pointerAnimation;
    
    private void Update() {
      if (senseManager.IsSensing) {
        objectiveManager.UpdateCurrentObjective();
        UpdatePointer();
        spriteRenderer.enabled = true;
        return;
      }

      spriteRenderer.enabled = false;
    }

    private void UpdatePointer() {
      var oldColor = spriteRenderer.color;
      spriteRenderer.color = new Color(oldColor.r, oldColor.g, oldColor.b, GetOpacity());
    }

    private float GetOpacity() {
      var currentObjective = objectiveManager.CurrentObjective;
      var objectivePosition = GetObjectivePosition(currentObjective);
      if (objectivePosition == null) {
        return 0f;
      }

      var objectiveVector = (Vector3) objectivePosition - player.PlayerTransform.position;
      return distanceOpacityCurve.Evaluate(objectiveVector.magnitude);
    }
    
    private IEnumerator FadeInPointer() {
      for (var i = 0f; i <= fadeTime; i += Time.deltaTime) {
        var spriteColor = spriteRenderer.color;
        spriteRenderer.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b,i / fadeTime * GetOpacity());
        yield return null;
      }
    }
    
    private IEnumerator FadeOutPointer() {
      var startOpacity = spriteRenderer.color.a;
      for (var i = 0f; i <= fadeTime; i += Time.deltaTime) {
        var spriteColor = spriteRenderer.color;
        spriteRenderer.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b,(1 - i / fadeTime) * startOpacity);
        yield return null;
      }
    }

    private Vector3? GetObjectivePosition(Objective currentObjective) {
      switch (currentObjective.objectiveType) {
        case ObjectiveType.FIND_OBJECTS:
          return objectiveTransformManager.GetTransformOfObject(currentObjective.objects[0])?.position;
        case ObjectiveType.CONVERSATION:
          return objectiveTransformManager.GetTransformOfCat(currentObjective.conversations[0])?.position;
        case ObjectiveType.USE_ENTRANCE:
          return objectiveTransformManager.GetTransformOfEntrance(currentObjective.entrances[0])?.position;
        default:
          return transform.position;
      }
    }
  }
}
