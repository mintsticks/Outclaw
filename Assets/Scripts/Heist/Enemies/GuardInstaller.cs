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

    [Header("Speeds")]
    [Tooltip("Speed when chasing or pathfinding.")]
    [SerializeField] private float activeSpeed;
    [Tooltip("Speed when patrolling.")]
    [SerializeField] private float passiveSpeed;

    public override void InstallBindings() {
      Container.BindInstance(visionCone)
        .WithId("Vision Cone")
        .AsSingle();

      Container.BindInstance(raycastLayers)
        .WithId("Raycast Layers")
        .AsSingle();

      Container.BindInstance(activeSpeed)
        .WithId("Active Speed");

      Container.BindInstance(passiveSpeed)
        .WithId("Passive Speed");
    }

  }
}
