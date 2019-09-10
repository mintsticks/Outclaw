using System;
using System.Collections;
using Outclaw.City;
using UnityEngine;

namespace Outclaw.City {
  public class InteractableCat : MonoBehaviour, Interactable {
    [SerializeField]
    private GameObject indicatorPrefab;

    [SerializeField]
    private GameObject heartPrefab;

    [SerializeField]
    private float fadeTime;

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private Indicator talkIndicator;
    
    private GameObject heart;
    private SpriteRenderer heartRenderer;
    private Transform parent;

    public void Awake() {
      talkIndicator.Initialize(GameManager.Instance.PlayerInstance.transform);
    }

    public void InRange() {
      talkIndicator.CreateIndicator();
      StartCoroutine(talkIndicator.FadeIn());
    }

    public void ExitRange() {
      StartCoroutine(talkIndicator.FadeOut());
    }

    public void Interact() {
      if (heart == null) {
        StartCoroutine(HandleHeart());
      }
    }

    private IEnumerator HandleHeart() {
      heart = Instantiate(heartPrefab, transform.position + offset, Quaternion.identity, transform);
      yield return new WaitForSeconds(fadeTime);
      Destroy(heart);
    }
  }
}