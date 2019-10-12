using UnityEngine;
using Zenject;

namespace City {
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