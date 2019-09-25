using UnityEngine;

namespace Outclaw {
  public interface ISoundManager {
    void PlaySFX(AudioClip clip);
    void PlayMusic(AudioClip clip);
    void PlayAmbientSound(AudioClip clip);
  }
  
  public class SoundManager : MonoBehaviour, ISoundManager {
    [SerializeField]
    private AudioSource sfxSource;

    [SerializeField]
    private AudioSource musicSource;

    [SerializeField]
    private AudioSource ambientSource;
    
    public void PlaySFX(AudioClip clip) {
      sfxSource.clip = clip;
      sfxSource.Play();
    }

    public void PlayMusic(AudioClip clip) {
      // don't replay if already playing
      if(musicSource.clip == clip){
        return;
      }
      musicSource.clip = clip;
      musicSource.Play();
    }

    public void PlayAmbientSound(AudioClip clip) {
      // don't replay if already playing
      if(ambientSource.clip == clip){
        return;
      }
      ambientSource.clip = clip;
      ambientSource.Play();
    }
  }
}