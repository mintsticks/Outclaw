using UnityEngine;
using Zenject;

namespace Outclaw.Heist {
  public class HeistMusic : MonoBehaviour {
    [SerializeField]
    private AudioClip backgroundMusic;

    [Inject]
    private ISoundManager soundManager;
    
    public void Awake() {
      soundManager.PlayMusic(backgroundMusic);  
    }
  }
}