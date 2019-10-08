using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw{
  public static class RandomUtility
  {
    // uses half of Marsaglia polar method to approx standard normal from uniforms
    public static float RandomStandardNormal(){

      // get 2 random values and sum of squares must be between 0 and 1
      float val1, val2, squareSum;
      do{
        val1 = Random.Range(-1f, 1f);
        val2 = Random.Range(-1f, 1f);
        squareSum = (val1 * val1) + (val2 * val2);
      }while(squareSum >= 1 || squareSum == 0);

      // Marsaglia polar method
      return val1 * Mathf.Sqrt(-2 * Mathf.Log(squareSum) / squareSum);
    }

    // takes a standard normal and transforms according to parameters
    public static float RandomNormal(float mean, float standardDeviation){
      return (RandomStandardNormal() - mean) / standardDeviation;
    }

    // shuffle by swapping with random index
    public static void ShuffleList<T>(List<T> list){
      for(int i = 0; i < list.Count; ++i){
        int j = Random.Range(0, list.Count - 1);
        T elem = list[i];
        list[i] = list[j];
        list[j] = elem;
      }
    }
  }
}
