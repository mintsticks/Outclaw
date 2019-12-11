using System.Collections;
using City;
using Outclaw.City;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Outclaw {
  public class ObjectivePointer : MonoBehaviour {
    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private Transform pointerParent;

    [SerializeField]
    private Transform iconParent;

    [SerializeField]
    private Transform followIcon;

    [SerializeField]
    private Image exclamation;

    [SerializeField]
    private Image followExclamation;
    
    [SerializeField]
    private Color minColor;

    [SerializeField]
    private Color maxColor;
    
    [SerializeField]
    private AnimationCurve distanceColorCurve =
      new AnimationCurve(new Keyframe(0, 1f), new Keyframe(10, 0f));

    [SerializeField]
    private float fadeTime;
    
    [Inject] 
    private IObjectiveManager objectiveManager;

    [Inject] 
    private IObjectiveTransformManager objectiveTransformManager;

    [Inject] 
    private ILocationManager locationManager;

    [Inject] 
    private ISpawnManager spawnManager;

    [Inject]
    private ISenseManager senseManager;

    [Inject]
    private IPlayer player;

    private IEnumerator currentAnimation;
    private bool isAnimating;
    private Camera main;
    private Quaternion constantIconRotation;
    
    // caching, done to minimize pathfinding calls from NextLocationTo()
    private Task lastTaskPointedAt;
    private Vector3? lastTaskPosition;

    private void Awake() {
      main = Camera.main;
      constantIconRotation = iconParent.rotation;
    }

    private void Update() {
      CheckSenseDown();
      CheckSense();
      CheckSenseUp();
    }

    private bool HasPointingTarget() {
      return GetTaskPosition(objectiveManager.CurrentTask) != null;
    }
    
    private void CheckSenseDown() {
      if (!senseManager.IsSenseDown || !HasPointingTarget()) {
        return;
      }
      
      
      StopCurrentAnimation();
      StartNewAnimation(FadeIn());
    }

    private void CheckSense() {
      if (!senseManager.IsSensing && !isAnimating) {
        return;
      }

      FindIntersection();
    }

    private void CheckSenseUp() {
      if (!senseManager.IsSenseUp || !HasPointingTarget()) {
        return;
      }

      StopCurrentAnimation();
      StartNewAnimation(FadeOut());
    }

    private void FindIntersection() {
      var currentTask = objectiveManager.CurrentTask;
      var objectivePosition = GetTaskPosition(currentTask);
      if (objectivePosition == null) {
        return;
      }

      UpdateColor();
      var playerOrigin = player.PlayerTransform.position;
      var playerDest = (Vector3) objectivePosition;
      var bounds = main.OrthographicBounds();
      
      var leftOrigin = bounds.min;
      var leftDest = new Vector2(bounds.min.x, bounds.max.y);
      if (CheckCameraSide(leftOrigin, leftDest, playerOrigin, playerDest, Vector2.left)) {
        return;
      }
     
      var rightOrigin = bounds.max;
      var rightDest = new Vector2(bounds.max.x, bounds.min.y);
      if (CheckCameraSide(rightOrigin, rightDest, playerOrigin, playerDest, Vector2.right)) {
        return;
      }
      
      var topOrigin = bounds.max;
      var topDest = new Vector2(bounds.min.x, bounds.max.y);
      if (CheckCameraSide(topOrigin, topDest, playerOrigin, playerDest, Vector2.up)) {
        return;
      }
      
      var botOrigin = bounds.min;
      var botDest = new Vector2(bounds.max.x, bounds.min.y);
      if (CheckCameraSide(botOrigin, botDest, playerOrigin, playerDest, Vector2.down)) {
        return;
      }
      
      ToggleFollowIcon(true);
      followIcon.position = main.WorldToScreenPoint(playerDest);
    }

    private void ToggleFollowIcon(bool toggle) {
      followIcon.gameObject.SetActive(toggle);
      pointerParent.gameObject.SetActive(!toggle);
    }

    private bool CheckCameraSide(Vector2 sideOrigin, 
      Vector2 sideDest, 
      Vector2 playerPos, 
      Vector2 objPos, 
      Vector2 rotationDirection) {
      var intersect = new Vector2();
      if (!LineHelperExt.FindLineIntersection(sideOrigin, sideDest, playerPos, objPos, ref intersect)) {
        return false;
      }
      ToggleFollowIcon(false);
      
      //TODO: cap the position of the indicator so it doesn't slide off screen at all
      pointerParent.position = main.WorldToScreenPoint(intersect);
      
      var rotation = Quaternion.LookRotation(Vector3.forward, rotationDirection);
      pointerParent.rotation = rotation;
      iconParent.rotation = constantIconRotation;
      return true;
    }

    private void UpdateColor() {
      var color = Color.Lerp(minColor, maxColor, GetColorIntensity());
      exclamation.color = color;
      followExclamation.color = color;
    }
    
    private float GetColorIntensity() {
      var currentTask = objectiveManager.CurrentTask;
      var objectivePosition = GetTaskPosition(currentTask);
      if (objectivePosition == null) {
        return 0f;
      }

      var objectiveVector = (Vector3) objectivePosition - player.PlayerTransform.position;
      return distanceColorCurve.Evaluate(objectiveVector.magnitude);
    }
    
    private IEnumerator FadeIn() {
      var startEffectAmount = canvasGroup.alpha;
      var changeEffectAmount = 1 - startEffectAmount;
      yield return Fade(startEffectAmount, changeEffectAmount);
    }
    
    private IEnumerator FadeOut() {
      var startEffectAmount = canvasGroup.alpha;
      var changeEffectAmount = -startEffectAmount;
      yield return Fade(startEffectAmount, changeEffectAmount);
    }
    
    private void StopCurrentAnimation() {
      if (currentAnimation == null) {
        return;
      }
      
      StopCoroutine(currentAnimation);
    }

    private void StartNewAnimation(IEnumerator animation) {
      currentAnimation = animation;
      StartCoroutine(currentAnimation);
    }
    
    private IEnumerator Fade(float startAlpha, float toAddAlpha) {
      isAnimating = true;
      for (var i = 0f; i <= fadeTime; i += Time.deltaTime) {
        canvasGroup.alpha = startAlpha + i / fadeTime * toAddAlpha;
        yield return null;
      }

      canvasGroup.alpha = startAlpha + toAddAlpha;
      isAnimating = false;
    }


    private bool IsPointableTask(Task task){
      if(task == null){
        Debug.LogWarning("No task to point get position of.");
        return false;
      }
      if(task.Location == null){
        Debug.LogWarning(task + " does not have a location set.");
        return false;
      }

      return true;
    }

    // if objective pointer lives beyond scenes, also need to check
    // if last cached value is from current location
    private bool IsPositionCached(Task task){
      return task == lastTaskPointedAt;
    }

    private Vector3? GetTaskPosition(Task task) {
      if(!IsPointableTask(task)){
        return null;
      }
      if(IsPositionCached(task)){
        return lastTaskPosition;
      }

      Vector3? objectivePosition = null;
      LocationData currentLocation = locationManager.CurrentLocation;
      // task is in another location, find an exit to get closer
      if (!task.Location.Equals(currentLocation))
      {
        LocationData nextScene = currentLocation.NextLocationTo(task.Location);
        if (nextScene == null) {
          Debug.LogWarning("Could not find location path between " 
            + currentLocation + " and " + task.Location);
          return null;
        }
        objectivePosition = spawnManager.GetExit(nextScene);
      } else { // task is in current location, find position in scene
        objectivePosition = objectiveTransformManager.GetTransformOfTask(task)?.position;
      }

      lastTaskPointedAt = task;
      lastTaskPosition = objectivePosition;
      return objectivePosition;
    }
  }
}
