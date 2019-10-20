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
    private Color spriteColor = new Color(1, 1, 1);
    
    public void Initialize(Transform _parent) {
      parent = _parent;
    }

    public Color SpriteColor {
      set {
        var oldAlpha = spriteColor.a;
        spriteColor = value;
        if (spriteRenderer != null) {
          spriteRenderer.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b,oldAlpha);
        }
      }
    }
    
    public IEnumerator FadeIn() {
      for (var i = 0f; i <= fadeTime; i += Time.deltaTime) {
        MaybeCreateIndicator();
        spriteRenderer.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b,i / fadeTime);
        yield return null;
      }
    }

    public IEnumerator FadeOut() {
      for (var i = fadeTime; i >= 0; i -= Time.deltaTime) {
        if (spriteRenderer == null) {
          yield break;;
        }
        spriteRenderer.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b,i / fadeTime);
        yield return null;
      }

      Destroy(indicator);
      spriteRenderer = null;
    }

    public void DestroyIndicator() {
      Destroy(indicator);
    }
    
    private void MaybeCreateIndicator() {
      if (spriteRenderer != null) {
        return;
      }
      CreateIndicator();
    }
    
    public void CreateIndicator() {
      Destroy(indicator);
      indicator = Instantiate(indicatorPrefab, parent.transform.position + offset, Quaternion.identity, parent.transform);
      spriteRenderer = indicator.GetComponent<SpriteRenderer>();
    }
  }
}