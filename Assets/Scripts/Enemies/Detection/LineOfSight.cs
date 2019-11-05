using Managers;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public class LineOfSight : MonoBehaviour {
    [SerializeField] private OnDetect onDetect = new OnDetect();

    [Inject] private IPlayerLitManager litManager;
    [Inject] private IHideablePlayer hideablePlayer;
    
    public void EnterAttention() {
      if (!litManager.IsLit || hideablePlayer.Hidden) {
        return;
      }
      onDetect.Invoke();
    }
  }
}