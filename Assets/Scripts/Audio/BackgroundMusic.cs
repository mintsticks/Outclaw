using UnityEngine;
using Zenject;

namespace Outclaw {
  public class BackgroundMusic : MonoBehaviour {
    [SerializeField]
    private AudioClip backgroundMusic;

    [SerializeField]
    private AudioClip ambientSound;

    [Inject]
    private ISoundManager soundManager;
    
    public void Awake() {
      soundManager.PlayMusic(backgroundMusic);
      soundManager.PlayAmbientSound(ambientSound);
    }
  }
}