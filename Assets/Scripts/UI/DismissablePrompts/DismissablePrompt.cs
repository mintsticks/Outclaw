using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Outclaw {
  public interface IDismissablePrompt {
    bool IsDismissable();
    IEnumerator DismissPrompt();
    void SetPromptText(string text);
  }

  public class DismissablePromptFactory : PlaceholderFactory<PromptType, IDismissablePrompt> { }

  public class CustomPromptFactory : IFactory<PromptType, IDismissablePrompt> {
    [Inject]
    private PromptSettings promptSettings;

    [Inject]
    private DiContainer container;

    public IDismissablePrompt Create(PromptType type) {
      return container.InstantiatePrefabForComponent<IDismissablePrompt>(GetTypePrefab(type));
    }

    private GameObject GetTypePrefab(PromptType type) {
      var prompt = promptSettings.promptPrefabs.FirstOrDefault(p => p.promptType == type);
      return prompt?.prefab;
    }
  }
  
  [Serializable]
  public class PromptSettings {
    public List<PromptPrefab> promptPrefabs;
  }

  [Serializable]
  public class PromptPrefab {
    public PromptType promptType;
    public GameObject prefab;
  }
  
  public enum PromptType {
    NONE,
    MOVE,
    JUMP,
    INTERACT
  }
}