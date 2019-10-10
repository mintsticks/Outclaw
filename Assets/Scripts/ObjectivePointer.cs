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
    [Inject] 
    private IPlayerInput playerInput;
    
    [Inject] 
    private IGameStateManager gameStateManager;

    [Inject] 
    private IObjectiveManager objectiveManager;

    [Inject] 
    private IObjectiveTransformManager objectiveTransformManager;
    
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private void Update() {
      EnablePointer();
    }

    private void UpdatePointer() {
      var currentObjective = objectiveManager.CurrentObjective;
      var objectivePosition = GetObjectivePosition(currentObjective);
      if (objectivePosition == null) {
        return;
      }
      
      var parentTransform = transform.parent;
      var objectiveVector = (Vector3) objectivePosition - parentTransform.position;
      parentTransform.rotation = Quaternion.LookRotation(Vector3.forward, objectiveVector);
      spriteRenderer.enabled = true;
    }

    private void EnablePointer() {
      if (playerInput.IsSense()) {
        objectiveManager.UpdateCurrentObjective();
        UpdatePointer();
        return;
      }

      spriteRenderer.enabled = false;
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
