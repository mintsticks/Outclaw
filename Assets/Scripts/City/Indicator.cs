using System.Collections;
using UnityEngine;

namespace Outclaw.City {
  public class Indicator : MonoBehaviour {
    [SerializeField]
    private GameObject indicatorPrefab;

    [SerializeField]
    private float fadeTime;

    [SerializeField]
    private Vector3 offset;

    private GameObject indicator;
    private SpriteRenderer spriteRenderer;
    private Transform parent;
    
    public void Initialize(Transform _parent) {
      parent = _parent;
    }
    
    public void CreateIndicator() {
      Destroy(indicator);
      indicator = Instantiate(indicatorPrefab, parent.transform.position + offset, Quaternion.identity, parent.transform);
      spriteRenderer = indicator.GetComponent<SpriteRenderer>();
    }

    public IEnumerator FadeIn() {
      for (var i = 0f; i <= fadeTime; i += Time.deltaTime) {
        if (spriteRenderer == null) {
          CreateIndicator();
        }

        spriteRenderer.color = new Color(1, 1, 1, i / fadeTime);
        yield return null;
      }
    }

    public IEnumerator FadeOut() {
      for (var i = fadeTime; i >= 0; i -= Time.deltaTime) {
        spriteRenderer.color = new Color(1, 1, 1, i / fadeTime);
        yield return null;
      }

      Destroy(indicator);
      spriteRenderer = null;
    }
  }
}