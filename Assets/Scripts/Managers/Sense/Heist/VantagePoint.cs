using System;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public class VantagePoint : MonoBehaviour {
    public Vector3 cameraPosition;
    public float cameraSize;

    [SerializeField] private Indicator indicator;
    
    public void ShowIndicator() {
      indicator.FadeIn();
    }
    
    public void HideIndicator() {
      indicator.FadeOut();
    }
  }

  public interface IVantagePointManager {
    bool InVantage { get; }
    void RegisterCurrentVantage(VantagePoint vantage);
    void ResetVantage();
    void ToCurrentVantageView();
    void ToCachedVantageView();
  }
  
  public class VantagePointManager : IInitializable, IVantagePointManager {
    private VantagePoint currentVantagePoint;
    private Camera main;

    private Vector3 cachedCameraPos;
    private float cachedCameraSize;
    private bool inVantage;
    
    public void Initialize() {
      main = Camera.main;
    }

    public bool InVantage => inVantage;

    public void RegisterCurrentVantage(VantagePoint vantagePoint) {
      currentVantagePoint = vantagePoint;
    }

    public void ResetVantage() {
      currentVantagePoint = null;
      if (!inVantage) {
        return;
      }
      ToCachedVantageView();
    }
    
    public void ToCurrentVantageView() {
      if (currentVantagePoint == null) {
        return;
      }
      
      inVantage = true;
      cachedCameraPos = main.transform.position;
      cachedCameraSize = main.orthographicSize;
      
      main.orthographicSize = currentVantagePoint.cameraSize;
      main.transform.position = currentVantagePoint.cameraPosition;
    }

    public void ToCachedVantageView() {
      if (!inVantage) {
        return;
      }
      inVantage = false;
      main.orthographicSize = cachedCameraSize;
      main.transform.position = cachedCameraPos;
    }
  }
}