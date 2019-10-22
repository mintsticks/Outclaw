using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public class DarkenOnSenseElement : MonoBehaviour {
    [Inject] private IHeistSenseManager senseManager;
    
    public SpriteRenderer spriteRenderer;
    public Color regularColor;
    public Color darkenedColor;
    
    private void Awake() {
      senseManager.RegisterElementToDarken(this);
    }
  }
}