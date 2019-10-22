using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public class ShowOnSenseElement : MonoBehaviour {
    [Inject] private IHeistSenseManager senseManager;
    
    public SpriteRenderer spriteRenderer;
    public Color regularColor = new Color(1, 1, 1, 0);
    public Color showColor = new Color(1, 1, 1, 1);
    
    private void Awake() {
      senseManager.RegisterElementToShow(this);
      gameObject.SetActive(false);
    }
  }
}