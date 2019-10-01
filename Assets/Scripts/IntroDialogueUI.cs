using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Outclaw {
  public class IntroDialogueUI : MonoBehaviour {

    [SerializeField] private float textSpeed = 0.025f;

    [SerializeField] private Text introText;

    [SerializeField] private string[] lines;

    [Inject] private IPlayerInput playerInput;

    public void Start()
    {
      StartCoroutine(PrintLines(lines));
    }

    public void Update()
    {
      
    }

    private IEnumerator PrintLines(string[] lines) {
      foreach (string line in lines)
      {
        yield return StartCoroutine(PrintLine(line));
      }

      SceneManager.LoadScene("Home");
    }
    private IEnumerator PrintLine(string line) {
      StringBuilder stringBuilder = new StringBuilder();
      introText.text = stringBuilder.ToString();
      yield return new WaitForSeconds(textSpeed);
      float time = 0.0f;
      foreach (char c in line)
      {
        while (time < textSpeed)
        {
          time += Time.deltaTime;

          if (playerInput.IsInteractDown())
          {
            introText.text = line;
            break;
          }
          
          yield return null;
        } 
        
        time = 0.0f;

        if (!playerInput.IsInteractDown())
        {
          stringBuilder.Append(c);
          introText.text = stringBuilder.ToString();
          yield return null;
        }
        else
        {
          introText.text = line;
          yield return null;
          break;
        }
        
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
