using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SenseAbility : MonoBehaviour {
  // actual effect
  [SerializeField]
  private GameObject visionField = null;

  private float origScale = 0f;

  [SerializeField]
  private float expandTime = 1f;

  [SerializeField]
  private float holdTime = 2f;

  [SerializeField]
  private float contractTime = 3f;

  [SerializeField]
  private float maxSizeScale = 3f;

  public Image senseImage;
  public Text senseText;

  // using
  [SerializeField]
  private float cooldown = 10f;

  public bool Useable { get; private set; }

  void Start() {
    origScale = visionField.transform.localScale.x;
    Useable = true;
    //senseImage.color = Color.white;
    //senseText.enabled = true;
  }

  public void UseAbility() {
    if (Useable) {
      StartCoroutine(Activate());
      StartCoroutine(Cooldown());
    }
  }

  private IEnumerator Cooldown() {
    Useable = false;
    //senseImage.color = Color.gray;
    //senseText.enabled = false;
    yield return new WaitForSeconds(cooldown);
    Useable = true;
    //senseImage.color = Color.white;
    //senseText.enabled = true;
  }

  private IEnumerator Activate() {
    yield return TransitionScale(1, maxSizeScale, expandTime);
    yield return new WaitForSeconds(holdTime);
    yield return TransitionScale(maxSizeScale, 1, contractTime);
  }

  private IEnumerator TransitionScale(float start, float end, float duration) {
    float timePassed = 0;
    while (timePassed < duration) {
      timePassed += Time.deltaTime;
      float scale = origScale * Mathf.Lerp(start, end, timePassed / duration);
      visionField.transform.localScale = new Vector3(scale, scale, scale);
      yield return null;
    }

    yield break;
  }
}