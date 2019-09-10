using UnityEngine;

namespace Outclaw.City {
  public class CameraBehavior : MonoBehaviour {
    [SerializeField]
    private Transform target;

    [SerializeField]
    private float smoothSpeed = .125f;

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private Vector2 minBound;

    [SerializeField]
    private Vector2 maxBound;
    
    void FixedUpdate() {
      var desiredPos = target.position + offset;
      var smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
      smoothedPos.x = Mathf.Clamp(smoothedPos.x, minBound.x, maxBound.x);
      smoothedPos.y = Mathf.Clamp(smoothedPos.y, minBound.y, maxBound.y);
      transform.position = smoothedPos;
    }
  }
}