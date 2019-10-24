using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw{
  [CreateAssetMenu(fileName = "Location Data", menuName = "Outclaw/Location Data")]
  public class LocationData : ScriptableObject
  {
    [SerializeField] private string sceneName;

    public string SceneName { get => sceneName; }
  }
}
