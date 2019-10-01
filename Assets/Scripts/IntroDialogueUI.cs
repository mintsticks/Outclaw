using System.Collections;
using System.Text;
using Outclaw.City;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Outclaw {
  public class IntroDialogueUI : MonoBehaviour {
    [SerializeField]
    private float textSpeed = 0.025f;

    [SerializeField]
    private Text introText;

    [SerializeField]
    private string[] lines;

    [Inject]
    private IPlayerInput playerInput;

    [Inject]
    private ISceneTransitionManager sceneTransitionManager;
    
    public void Start() {
      StartCoroutine(PrintLines(lines));
    }

    private IEnumerator PrintLines(string[] lines) {
      foreach (var line in lines) {
        yield return StartCoroutine(PrintLine(line));
      }

      sceneTransitionManager.TransitionToScene("Home");
    }

    private IEnumerator PrintLine(string line) {
      line = line.Replace(".n", ".\n");
      StringBuilder stringBuilder = new StringBuilder();
      introText.text = stringBuilder.ToString();
      yield return new WaitForSeconds(textSpeed);
      float time = 0.0f;
      var charSpeed = textSpeed;
      foreach (char c in line) {
        if (!playerInput.IsInteractDown()) {
          stringBuilder.Append(c);
          introText.text = stringBuilder.ToString();
          yield return null;
        } else {
          introText.text = line;
          yield return null;
          break;
        }
        if (c == '.') {
          charSpeed = 1;
        } else {
          charSpeed = textSpeed;
        }
        while (time < charSpeed) {
          time += Time.deltaTime;
          if (playerInput.IsInteractDown()) {
            introText.text = line;
            break;
          }

          yield return null;
        }

        time = 0.0f;
      }

      yield return NextLine();
    }

    private IEnumerator NextLine() {
      while (!playerInput.IsInteractDown()) {
        yield return null;
      }
    }
  }
}