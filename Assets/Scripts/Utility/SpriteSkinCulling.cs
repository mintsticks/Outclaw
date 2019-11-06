using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist{
  public class SpriteSkinCulling : MonoBehaviour
  {
    [Tooltip("This distance outside of camera bounds will hide this sprite.")]
    [SerializeField] private float distanceToHide = 5f;

    private SpriteRenderer[] renderers;

    // Start is called before the first frame update
    void Start()
    {
      // assume all GameObjects with SpriteSkin have SpriteRenderer
      // since SpriteSkin can't be accessed directly
      renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
      Bounds camBounds = Camera.main.OrthographicBounds();
      camBounds.Expand(new Vector3(distanceToHide, distanceToHide, 100));
      Toggle(camBounds.Contains(transform.position));
    }

    private void Toggle(bool on){
      foreach(SpriteRenderer rend in renderers){
        rend.gameObject.SetActive(on);
      }
    }
  }
}
