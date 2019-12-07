using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw.Testing{
  public class LocationDataGraphTest : MonoBehaviour
  {

    [SerializeField] private List<Test> tests;

    // Start is called before the first frame update
    void Start()
    {
      foreach(Test test in tests){
        RunTest(test);
      }
    }


    private void RunTest(Test test){
      LocationData actualResult = test.source.NextLocationTo(test.dest);
      Debug.Log(((actualResult == test.expectedResult) ? "Pass" : "**FAIL**")
        + "\nActual: " + actualResult 
        + "\nExpected: " + test.expectedResult);
    }

    [System.Serializable]
    public class Test{
      public LocationData source;
      public LocationData dest;
      public LocationData expectedResult;
    }
  }
}
