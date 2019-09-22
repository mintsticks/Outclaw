#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist{
  public class GuardInstaller : MonoInstaller
  {

    [SerializeField] private GameObject visionCone;
    [SerializeField] private LayerMask raycastLayers;

    public override void InstallBindings() {
      Container.BindInstance(visionCone)
        .WithId("Vision Cone")
        .AsSingle();

      Container.BindInstance(raycastLayers)
        .WithId("Raycast Layers");
    }

  }
}
