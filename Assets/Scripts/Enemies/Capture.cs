using System.Collections;
using System.Collections.Generic;
using Outclaw.City;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist{
  public class Capture : MonoBehaviour {
    [SerializeField] private SpriteRenderer alertSprite;
    [SerializeField] private float fadeTime = .25f;
    [Inject] private ICapturedMenu captureMenu;
    [Inject] private IPlayer player;

    public void CapturePlayer() {
      StartCoroutine(CaptureCoroutine());
    }

    private IEnumerator CaptureCoroutine() {
      player.InputDisabled = true;
      yield return FadeAlertIn();
      yield return new WaitForSeconds(.5f);
      captureMenu.Show();
    }

    private IEnumerator FadeAlertIn() {
      alertSprite.gameObject.SetActive(true);
      var spriteColor = alertSprite.color;
      for (var i = 0f; i <= fadeTime; i += Time.deltaTime) {
        alertSprite.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b,i / fadeTime);
        yield return null;
      }
    }
  }
}
