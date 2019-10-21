using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn;
using Yarn.Unity;
using Zenject;

namespace Outclaw.City {
  public interface IDialogueManager {
    void SetDialogue(TextAsset[] text);
    void StartDialogue(Action onComplete = null);
    bool IsDialogueRunning { get; }
    void SetBubbleParent(Transform parent);
    void SetDialogueType(DialogueType type);
  }
  
  public class DialogueManager : MonoBehaviour, IDialogueManager {
    [SerializeField]
    private DialogueRunner runner;
    
    [SerializeField]
    private CatDialogueUI uiBehaviour;

    [SerializeField]
    private VariableStorageBehaviour storageBehaviour;

    [Inject] 
    private IPlayer player;
    
    public void SetDialogue(TextAsset[] text) {
      runner.SourceText = text;
    }

    public void SetDialogueType(DialogueType type) {
      uiBehaviour.DialogueType = type;
    }

    public void StartDialogue(Action onComplete = null) {
      uiBehaviour.OnDialogueComplete = onComplete + EnableInput;
      player.InputDisabled = true;
      runner.StartDialogue();
    }

    private void EnableInput() {
      player.InputDisabled = false;
    }
    
    public bool IsDialogueRunning => runner.isDialogueRunning;
    
    public void SetBubbleParent(Transform parent) {
      uiBehaviour.BubbleParent = parent;
    }
  }
}