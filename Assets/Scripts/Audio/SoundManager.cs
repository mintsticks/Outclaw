using System.Collections;
using UnityEngine;
using Outclaw.ManagedRoutine;

namespace Outclaw {
  public interface ISoundManager {
    void PlaySFX(AudioClip clip);
    void PlaySFX(AudioClip clip, bool loop);
    void StopSFX();

    void PlayMusic(AudioClip clip);
    void PlayAmbientSound(AudioClip clip);

    void SubdueBackground();
    void UnsubdueBackground();
  }
  
  public class SoundManager : MonoBehaviour, ISoundManager {
    [Header("Sound Effects")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Music")]
    [SerializeField] private AudioSource currentMusicSource;
    [SerializeField] private AudioSource nextMusicSource;
    private Coroutine musicCrossfade;
    private ManagedCoroutine<AudioSource, AudioSource> musicCrossfadeRoutine;

    [Header("Ambient")]
    [SerializeField] private AudioSource currentAmbientSource;
    [SerializeField] private AudioSource nextAmbientSource;
    private Coroutine ambientCrossfade;
    private ManagedCoroutine<AudioSource, AudioSource> ambientCrossfadeRoutine;

    [Header("Extra Effects")]
    [SerializeField] private float crossfadeDuration = .5f;
    [SerializeField] private float timeToSubdue = .5f;
    [SerializeField] [Range(0, 1)] private float subduedVolume = .5f;
    private ManagedCoroutine<float> subdueBackgroundRoutine;

    void Awake(){
      musicCrossfadeRoutine = new ManagedCoroutine<AudioSource, AudioSource>(this, Crossfade);
      ambientCrossfadeRoutine = new ManagedCoroutine<AudioSource, AudioSource>(this, Crossfade);
      subdueBackgroundRoutine = new ManagedCoroutine<float>(this, LerpBackgroundVolume);
    }

    // need to have separeate PlaySFX because default values don't count as
    //   fufilling other function signitures
    public void PlaySFX(AudioClip clip){
      PlaySFX(clip, false);
    }

    public void PlaySFX(AudioClip clip, bool loop) {
      sfxSource.clip = clip;
      sfxSource.loop = loop;
      sfxSource.Play();
    }

    public void StopSFX(){
      sfxSource.Stop();
    }

    public void PlayMusic(AudioClip clip) {
      // don't replay if already playing
      if(currentMusicSource.clip == clip) {
        return;
      }

      StartCrossfade(clip, musicCrossfadeRoutine, ref currentMusicSource, 
        ref nextMusicSource);
    }

    public void PlayAmbientSound(AudioClip clip) {
      // don't replay if already playing
      if(currentAmbientSource.clip == clip) {
        return;
      }

      StartCrossfade(clip, ambientCrossfadeRoutine, ref currentAmbientSource, 
        ref nextAmbientSource);
    }

    private void StartCrossfade(AudioClip clip, 
        ManagedCoroutine<AudioSource, AudioSource> crossfade, 
        ref AudioSource current, ref AudioSource next) {

      // stop any previous crossfade
      crossfade.StopCoroutine();

      // start up crossfade
      AudioSource entering = next;
      AudioSource exiting = current;
      entering.clip = clip;
      entering.Play();
      crossfade.StartCoroutine(entering, exiting);

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

    public void SubdueBackground(){
      subdueBackgroundRoutine.StopCoroutine();
      subdueBackgroundRoutine.StartCoroutine(subduedVolume);
    }

    public void UnsubdueBackground(){
      subdueBackgroundRoutine.StopCoroutine();
      subdueBackgroundRoutine.StartCoroutine(1f);
    }

    private IEnumerator LerpBackgroundVolume(float endVolume){
      float origMusicVolume = currentMusicSource.volume;
      float origAmbientVolume = currentAmbientSource.volume;
      for(float time = 0; time < timeToSubdue; time += Time.deltaTime){
        currentMusicSource.volume = Mathf.Lerp(origMusicVolume, endVolume, time / timeToSubdue);
        currentAmbientSource.volume = Mathf.Lerp(origAmbientVolume, endVolume, time / timeToSubdue);
        yield return null;
      }

      // set directly incase lerp doesn't fully match
      currentMusicSource.volume = endVolume;
      currentAmbientSource.volume = endVolume;
    }
  }
}