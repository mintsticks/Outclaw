using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Outclaw{
  public class SpriteSkinCulling : MonoBehaviour
  {
    [Tooltip("This distance outside of camera bounds will hide this sprite.")]
    [SerializeField] private float distanceToHide = 5f;
    [SerializeField] private Bounds visualBounds;
    private Vector3 boundOffset;

    private SpriteRenderer[] renderers;

    public Bounds VisualBounds => visualBounds;

    void Start()
    {
      // assume all GameObjects with SpriteSkin have SpriteRenderer
      // since SpriteSkin can't be accessed directly
      renderers = GetComponentsInChildren<SpriteRenderer>();

      Vector3 scaledSize = 2 * transform.TransformVector(visualBounds.extents);
      scaledSize.x = Mathf.Abs(scaledSize.x);
      scaledSize.y = Mathf.Abs(scaledSize.y);
      scaledSize.z = Mathf.Abs(scaledSize.z);

      visualBounds = new Bounds(transform.TransformPoint(visualBounds.center), 
        scaledSize);
      visualBounds.Expand(new Vector3(0, 0, 100));
      boundOffset = visualBounds.center - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
      Bounds camBounds = Camera.main.OrthographicBounds();
      camBounds.Expand(new Vector3(distanceToHide, distanceToHide, 100));
      visualBounds.center = transform.position + boundOffset;

      Debug.DrawLine(visualBounds.min, visualBounds.max, Color.magenta);
      Toggle(camBounds.Intersects(visualBounds));
    }

    private void Toggle(bool on){
      foreach(SpriteRenderer rend in renderers){
        rend.gameObject.SetActive(on);
      }
    }
  }
}
