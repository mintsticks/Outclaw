using System.Collections;
using System.Collections.Generic;
using Outclaw.Heist;
using UnityEngine;
using Zenject;

namespace Outclaw {

  public class ObjectivePointer : MonoBehaviour {

    [Inject] private IObjectiveManager objectiveManager;

  }
  
}
