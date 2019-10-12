using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace City {
  public interface ISenseManager {
    IEnumerator GreySprites();
    IEnumerator UngreySprites();
    void RegisterSpriteToGrey(SpriteRenderer spriteRenderer);
  }
  
  public class SenseManager : MonoBehaviour, ISenseManager {
    [SerializeField]
    private float senseDelay;

    private float animationFreq = .02f;
    
    [SerializeField]
    private List<SpriteRenderer> spritesToGrey;
   
    [SerializeField]
    private List<SpriteRenderer> spritesToHighlight;

    public void RegisterSpriteToGrey(SpriteRenderer spriteRenderer) {
      spritesToGrey.Add(spriteRenderer);
    }
    
    public IEnumerator GreySprites() {
      if (spritesToGrey.Count <= 0) {
        yield break;
      }

      var startEffectAmount = spritesToGrey[0].material.GetFloat("_EffectAmount");
      var addEffectAmount = 1 - startEffectAmount;
      for (var i = 0f; i < senseDelay; i += animationFreq) {
        foreach (var sprite in spritesToGrey) {
          sprite.material.SetFloat("_EffectAmount", startEffectAmount + i / senseDelay * addEffectAmount);
        }
        yield return new WaitForSeconds(animationFreq);
      }
    }

    public IEnumerator UngreySprites() {
      if (spritesToGrey.Count <= 0) {
        yield break;
      }
      var startEffectAmount = spritesToGrey[0].material.GetFloat("_EffectAmount");
      for (var i = 0f; i < senseDelay; i += animationFreq) {
        foreach (var sprite in spritesToGrey) {
          sprite.material.SetFloat("_EffectAmount", startEffectAmount - i / senseDelay * startEffectAmount);
        }
        yield return new WaitForSeconds(animationFreq);
      }
    }
  }
}