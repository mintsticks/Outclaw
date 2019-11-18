using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw{
  public class PlayerAnimEffects : MonoBehaviour
  {
    [SerializeField] private AudioSource landingSound;

    public void Landing(){
      landingSound?.Play();
    }
  }
}
