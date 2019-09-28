#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Outclaw.Heist{
  public class RegionBlur : MonoBehaviour
  {
    [SerializeField] private GameObject blurParent;

    [Inject] private Outclaw.City.IPlayer player;

    void Start(){
      Blur();
    }

    private void Blur(){
      blurParent.SetActive(true);
    }

    private void Unblur(){
      blurParent.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other){
      if(other.gameObject == player.PlayerTransform.gameObject){
        Unblur();
      }
    }

    void OnTriggerExit2D(Collider2D other){
      if(other.gameObject == player.PlayerTransform.gameObject){
        Blur();
      }
    }
  }
}
