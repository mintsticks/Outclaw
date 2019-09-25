using System.Collections;
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

    [Header("Music")]
    [SerializeField]
    private AudioSource currentMusicSource;

    [SerializeField]
    private AudioSource nextMusicSource;

    private Coroutine musicCrossfade;

    [Header("Ambient")]
    [SerializeField]
    private AudioSource currentAmbientSource;

    [SerializeField]
    private AudioSource nextAmbientSource;

    private Coroutine ambientCrossfade;

    [SerializeField]
    private float crossfadeDuration = .5f;
    
    public void PlaySFX(AudioClip clip) {
      sfxSource.clip = clip;
      sfxSource.Play();
    }

    public void PlayMusic(AudioClip clip) {
      // don't replay if already playing
      if(currentMusicSource.clip == clip) {
        return;
      }

      StartCrossfade(clip, ref musicCrossfade, ref currentMusicSource, 
        ref nextMusicSource);
    }

    public void PlayAmbientSound(AudioClip clip) {
      // don't replay if already playing
      if(currentAmbientSource.clip == clip) {
        return;
      }

      StartCrossfade(clip, ref ambientCrossfade, ref currentAmbientSource, 
        ref nextAmbientSource);
    }

    private void StartCrossfade(AudioClip clip, ref Coroutine crossfade, 
        ref AudioSource current, ref AudioSource next) {

      // stop any previous crossfade
      if(crossfade != null) {
        StopCoroutine(crossfade);
      }

      // start up crossfade
      AudioSource entering = next;
      AudioSource exiting = current;
      entering.clip = clip;
      entering.Play();
      crossfade = StartCoroutine(Crossfade(entering, exiting));

      // swap values
      current = entering;
      next = exiting;
    }

    private IEnumerator Crossfade(AudioSource entering, AudioSource exiting) {

      float exitStartVolume = exiting.volume;
      float totalTime = 0;
      while(totalTime < crossfadeDuration) {
        totalTime += Time.deltaTime;
        exiting.volume = Mathf.Lerp(exitStartVolume, 0, totalTime / crossfadeDuration);
        entering.volume = Mathf.Lerp(0, 1, totalTime / crossfadeDuration);
        yield return null;
      }

      exiting.Stop();
      yield break;
    }
  }
}