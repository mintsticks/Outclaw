using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Outclaw.Heist{
  public class Capture : MonoBehaviour
  {
    [Inject] private ICapturedMenu captureMenu;

    public void CapturePlayer(){
      captureMenu.Show();
    }
  }
}
