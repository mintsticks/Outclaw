using System;
using System.Collections.Generic;
using System.Linq;
using Outclaw;
using UnityEngine;
using Utility;
using Zenject;

namespace UI.DismissablePrompts {
  public interface IPlatformDependentCanvasComponent {
    CanvasGroup CanvasGroup { get; }
  }

  public class PlatformDependentCanvasComponent : MonoBehaviour, IPlatformDependentCanvasComponent {
    [SerializeField] private CanvasGroup canvasGroup;
    
    public CanvasGroup CanvasGroup => canvasGroup;
  }

  public class PlatformDependentCanvasFactory : PlaceholderFactory<PlatformDependentComponent, IPlatformDependentCanvasComponent> { }

  public class CustomCanvasFactory : IFactory<PlatformDependentComponent, IPlatformDependentCanvasComponent> {
    [Inject] private DiContainer container;

    public IPlatformDependentCanvasComponent Create(PlatformDependentComponent component) {
      return container.InstantiatePrefabForComponent<IPlatformDependentCanvasComponent>(GetPrefab(component));
    }

    private GameObject GetPrefab(PlatformDependentComponent component) {
      return component.platformData.FirstOrDefault(p => p.platform == PlatformUtil.GetPlatform())?.prefab;
    }
  }
}