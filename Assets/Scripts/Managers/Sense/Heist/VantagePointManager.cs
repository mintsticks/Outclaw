using System.Collections;
using Outclaw.City;
using UnityEngine;
using Utility;
using Zenject;

namespace Outclaw.Heist {

  public interface IVantagePointManager {
    bool InVantage { get; }
    void RegisterCurrentVantage(VantagePoint vantage);
    void ResetVantage();
    void ToVantageView();
    void ExitVantageView();
  }
  
  public class VantagePointManager : MonoBehaviour, IVantagePointManager {
    [SerializeField] private AnimationWrapper animationWrapper;
    [SerializeField] private float shiftToTime = .5f;
    [SerializeField] private float shiftBackTime = .25f;
    [SerializeField] private AnimationCurve shiftToAnim;
    
    [Inject] private ICameraBehavior cameraBehavior;

    private VantagePoint currentVantagePoint;
    private Camera main;
    
    private float defaultCameraSize;
    private bool inVantage;
    
    public bool InVantage => inVantage;
    private float animationProgress;
    
    public void Awake() {
      main = Camera.main;
      defaultCameraSize = main.orthographicSize;
    }
    
    public void RegisterCurrentVantage(VantagePoint vantagePoint) {
      vantagePoint.ShowIndicator();
      currentVantagePoint = vantagePoint;
    }

    public void ResetVantage() {
      if (currentVantagePoint != null) {
        currentVantagePoint.HideIndicator();
      }
      currentVantagePoint = null;
      if (!inVantage) {
        return;
      }
      ExitVantageView();
    }
    
    public void ToVantageView() {
      if (currentVantagePoint == null) {
        return;
      }
      
      inVantage = true;
      currentVantagePoint.HideIndicator();
      currentVantagePoint.UseVantage();
      cameraBehavior.ShouldFollow = false;
      animationWrapper.StartNewAnimation(UpdateCamera(currentVantagePoint.cameraPosition,
        currentVantagePoint.cameraSize, false, shiftToTime));
    }

    public void ExitVantageView() {
      if (!inVantage) {
        return;
      }

      if (currentVantagePoint != null) {
        currentVantagePoint.ShowIndicator();
      }
      
      inVantage = false;
      cameraBehavior.ShouldFollow = true;
      animationWrapper.StartNewAnimation(ReturnCamera(defaultCameraSize,
        shiftBackTime));
    }
    
    private IEnumerator UpdateCamera(Vector3 destPos, float destSize, bool shouldFollow, float shiftTime) {
      var startPos = main.transform.position;
      var startSize = main.orthographicSize;
      var changePos = destPos - startPos;
      var changeSize = destSize - startSize;
      
      for (var i = 0f; i < shiftTime; i += GlobalConstants.ANIMATION_FREQ) {
        animationProgress = i / shiftTime;
        main.transform.position = startPos + changePos * shiftToAnim.Evaluate(animationProgress);
        main.orthographicSize = startSize + changeSize * shiftToAnim.Evaluate(animationProgress);
        yield return new WaitForSeconds(GlobalConstants.ANIMATION_FREQ);
      }

      main.transform.position = destPos;
      main.orthographicSize = destSize;
      cameraBehavior.ShouldFollow = shouldFollow;
    }

    private IEnumerator ReturnCamera(float destSize, float shiftTime) {
      var startSize = main.orthographicSize;
      var changeSize = destSize - startSize;
      
      for (var i = 0f; i < shiftTime; i += GlobalConstants.ANIMATION_FREQ) {
        animationProgress = i / shiftTime;
        main.orthographicSize = startSize + changeSize * shiftToAnim.Evaluate(animationProgress);
        yield return new WaitForSeconds(GlobalConstants.ANIMATION_FREQ);
      }

      main.orthographicSize = destSize;
    }
  }
}