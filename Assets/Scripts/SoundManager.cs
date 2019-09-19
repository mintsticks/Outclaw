using UnityEngine;

namespace Outclaw {
  public interface ISoundManager {
    void PlaySFX(AudioClip clip);
    void PlayMusic(AudioClip clip);
  }
  
  public class SoundManager : MonoBehaviour, ISoundManager {
    [SerializeField]
    private AudioSource sfxSource;

    [SerializeField]
    private AudioSource musicSource;
    
    public void PlaySFX(AudioClip clip) {
      sfxSource.clip = clip;
      sfxSource.Play();
    }

    public void PlayMusic(AudioClip clip) {
      musicSource.clip = clip;
      musicSource.Play();
    }
  }
}