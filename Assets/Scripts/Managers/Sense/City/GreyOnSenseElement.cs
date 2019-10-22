using UnityEngine;
using Zenject;

namespace Outclaw.City {
  public class GreyOnSenseElement : MonoBehaviour {
    [Inject]
    private ISenseManager senseManager;

    [SerializeField]
    private SpriteRenderer spriteRenderer;
    
    private void Awake() {
      senseManager.RegisterSpriteToGrey(spriteRenderer);
    }
  }
}