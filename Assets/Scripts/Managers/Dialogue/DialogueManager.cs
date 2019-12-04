using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn;
using Yarn.Unity;
using Zenject;

namespace Outclaw.City {
  public interface IDialogueManager {
    bool IsDialogueRunning { get; }
    
    void StartDialogue(TextAsset[] text,
      DialogueType type,
      Transform parent,
      ObjectiveInteractable currentInteractable = null,
      Action onComplete = null);
  }

  public class DialogueManager : MonoBehaviour, IDialogueManager {
    [SerializeField] private DialogueRunner runner;
    [SerializeField] private CatDialogueUI uiBehaviour;
    [SerializeField] private VariableStorageBehaviour storageBehaviour;

    [Inject] private IPlayer player;
    [Inject] private ICameraBehavior cameraBehavior;
    
    private bool queuedDialogue;
    
    
    public bool IsDialogueRunning => runner.isDialogueRunning || queuedDialogue;
    
    public void StartDialogue(TextAsset[] text,
      DialogueType type,
      Transform parent,
      ObjectiveInteractable currentInteractable = null,
      Action onComplete = null) {
      if (queuedDialogue) {
        return;
      }
      runner.SourceText = text;
      uiBehaviour.BubbleParent = parent;
      uiBehaviour.CurrentInteractable = currentInteractable;
      uiBehaviour.OnDialogueComplete = onComplete + EnableInput;
      StartCoroutine(QueueDialogue());
    }

    
    private IEnumerator QueueDialogue() {
      queuedDialogue = true;
      cameraBehavior.ShouldFollow = false;
      player.InputDisabled = true;
      while (!player.Velocity.IsZero() || !player.IsGrounded) {
        yield return null;
      }
      
      runner.StartDialogue();
      queuedDialogue = false;
    }
    
    private void EnableInput() {
      player.InputDisabled = false;
      cameraBehavior.ShouldFollow = true;
    }
  }
}