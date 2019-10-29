using System.Collections;
using System.Collections.Generic;
using Outclaw.City;
using UnityEngine;
using Zenject;
using Outclaw.ManagedRoutine;

namespace Outclaw.Heist{
  public class Capture : MonoBehaviour {

    [Header("Awareness")]
    [SerializeField] private float maxAwareness = 1f;
    private float awareness = 0;
    [Tooltip("Awareness per second")]
    [SerializeField] private float awarenessIncreaseRate = 1f;
    [Tooltip("Awareness per second")]
    [SerializeField] private float awarenessDecreaseRate = 2f;
    [SerializeField] private SpriteRenderer alertSprite;

    [Inject] private ICapturedMenu captureMenu;
    [Inject] private IPlayer player;

    private ManagedCoroutine captureRoutine;
    private ManagedCoroutine fadeOutRoutine;

    void Awake(){
      captureRoutine = new ManagedCoroutine(this, CaptureCoroutine);
      fadeOutRoutine = new ManagedCoroutine(this, FadeAlertOut);
    }

    public void CapturePlayer() {
      fadeOutRoutine.StopCoroutine();
      captureRoutine.StartCoroutine();
    }

    public void CancelCapture(){
      captureRoutine.StopCoroutine();
      fadeOutRoutine.StartCoroutine();
    }

    public void CapturePlayerImmediate(){
      captureMenu.Show();
    }

    private IEnumerator FadeAlertOut(){
      var spriteColor = alertSprite.color;
      while (awareness > 0) {
        awareness -= awarenessDecreaseRate * Time.deltaTime;
        alertSprite.color = new Color(spriteColor.r, spriteColor.g, 
          spriteColor.b, awareness / maxAwareness);
        yield return null;
      }
      alertSprite.gameObject.SetActive(false);
    }

    private IEnumerator CaptureCoroutine() {
      yield return FadeAlertIn();
      player.InputDisabled = true;
      yield return new WaitForSeconds(.5f);
      captureMenu.Show();
    }

    private IEnumerator FadeAlertIn() {
      alertSprite.gameObject.SetActive(true);
      var spriteColor = alertSprite.color;
      while (awareness < maxAwareness) {
        awareness += awarenessIncreaseRate * Time.deltaTime;
        alertSprite.color = new Color(spriteColor.r, spriteColor.g, 
          spriteColor.b, awareness / maxAwareness);
        yield return null;
      }
    }
  }
}
