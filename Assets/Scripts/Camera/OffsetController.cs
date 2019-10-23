using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public class OffsetController : MonoBehaviour {
    [SerializeField] private AnimationCurve xOffsetForPosition;
    [SerializeField] private AnimationCurve yOffsetForPosition;

    [Inject] private ICameraBehavior cameraBehavior;
    [Inject] private IPlayer player;

    public void Update() {
      var position = player.PlayerTransform.position;
      var xOffset = xOffsetForPosition.Evaluate(position.x);
      var yOffset = yOffsetForPosition.Evaluate(position.x);
      cameraBehavior.Offset = new Vector3(xOffset, yOffset, cameraBehavior.Offset.z);
    }
  }
}