using System;
using Outclaw;
using UnityEngine;
using Zenject;

namespace UI.DismissablePrompts {
  public class PlatformDependentComponentContainer : MonoBehaviour {
    [SerializeField] private PlatformDependentComponent platformDependentComponent;
    [SerializeField] private Transform parent;
    
    [Inject] private PlatformDependentCanvasFactory canvasFactory;

    private CanvasGroup canvasGroup;

    public CanvasGroup CanvasGroup => canvasGroup;

    private void Awake() {
      if (platformDependentComponent == null) {
        return;
      }
      var canvasComponent = canvasFactory.Create(platformDependentComponent);
      canvasComponent.CanvasGroup.transform.SetParent(parent, false);
      canvasGroup = canvasComponent.CanvasGroup;
    }
  }
}