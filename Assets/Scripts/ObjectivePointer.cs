using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using City;
using ModestTree;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Outclaw {

  public class ObjectivePointer : MonoBehaviour {

    [Inject] private IPlayerInput playerInput;
    
    [Inject] private IGameStateManager gameStateManager;

    [Inject] private IObjectiveManager objectiveManager;

    [Inject] private IObjectiveTransformManager objectiveTransformManager;
    
    private SpriteRenderer spriteRenderer;

    private void Awake() {
      spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
      spriteRenderer.enabled = false;
    }

    private void Update()
    {
      EnablePointer();
    }

    private void UpdatePointer()
    {
      var currentObjective = objectiveManager.CurrentObjective;
      
      //get interactable position
      Vector3 objectivePosition = GetObjectivePosition(currentObjective);
      //get things for math
      var parentTransform = transform.parent;
      var objectiveVector = objectivePosition - parentTransform.position;
      //rotation
      parentTransform.rotation = Quaternion.LookRotation(Vector3.forward, objectiveVector);
    }

    private void EnablePointer() {
      if (playerInput.IsSense()) {
        spriteRenderer.enabled = true;
        UpdatePointer();
        return;
      }

      spriteRenderer.enabled = false;
    }

    public LocationType GetCurrentLocationType() {
      switch (SceneManager.GetActiveScene().name) {
        case "Home":
          return LocationType.HOME;
        case "HomeTest":
          return LocationType.HOME;
        case "Main":
          return LocationType.MAIN;
        case "CafeBottom":
          return LocationType.CAFEBOTTOM;
        case "CafeTop":
          return LocationType.CAFETOP;
        case "Park":
          return LocationType.PARK;
        default:
          return LocationType.NONE;
      }
    }

    public Vector3 GetObjectivePosition(Objective currentObjective) {
      //compare location to scene
      if (currentObjective.location != GetCurrentLocationType()) {
        return objectiveTransformManager.GetTransformOfLocation(GetCurrentLocationType()).position;
      }

      switch (currentObjective.objectiveType) {
        case ObjectiveType.FIND_OBJECTS:
          return objectiveTransformManager.GetTransformOfObject(currentObjective.objects[0]).position;
        case ObjectiveType.CONVERSATION:
          return objectiveTransformManager.GetTransformOfCat(currentObjective.conversations[0]).position;
        default:
          return transform.position;
      }
    }
  }
}
