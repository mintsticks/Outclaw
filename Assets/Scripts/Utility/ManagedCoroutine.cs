using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outclaw.ManagedRoutine {
  public abstract class ManagedCoroutineBase
  {
    protected MonoBehaviour script;
    protected Coroutine routine = null;

    public ManagedCoroutineBase(MonoBehaviour script) {
      this.script = script;
    }

    public void StopCoroutine(){
      if(routine != null){
        script.StopCoroutine(routine);
        routine = null;
      }
    }

    protected IEnumerator NullOnComplete(IEnumerator enumerator){
      yield return enumerator;
      routine = null;
      yield break;
    }

    public bool IsRunning
    {
      get { return routine != null; }
    }
  }

  public class ManagedCoroutine : ManagedCoroutineBase{

    private Func<IEnumerator> function;

    public ManagedCoroutine(MonoBehaviour script, Func<IEnumerator> function) 
        : base(script){

      this.function = function;
    }

    public void StartCoroutine(){
      if(routine == null){
        routine = script.StartCoroutine(NullOnComplete(function()));
      }
    }
  }

  public class ManagedCoroutine<T> : ManagedCoroutineBase{

    private Func<T, IEnumerator> function;

    public ManagedCoroutine(MonoBehaviour script, Func<T, IEnumerator> function) 
        : base(script){

      this.function = function;
    }

    public void StartCoroutine(T arg){
      if(routine == null){
        routine = script.StartCoroutine(NullOnComplete(function(arg)));
      }
    }
  }

  public class ManagedCoroutine<T1, T2> : ManagedCoroutineBase{

    private Func<T1, T2, IEnumerator> function;

    public ManagedCoroutine(MonoBehaviour script, Func<T1, T2, IEnumerator> function) 
        : base(script){

      this.function = function;
    }

    public void StartCoroutine(T1 arg1, T2 arg2){
      if(routine == null){
        routine = script.StartCoroutine(NullOnComplete(function(arg1, arg2)));
      }
    }
  }

  public class ManagedCoroutine<T1, T2, T3> : ManagedCoroutineBase{

    private Func<T1, T2, T3, IEnumerator> function;

    public ManagedCoroutine(MonoBehaviour script, Func<T1, T2, T3, IEnumerator> function) 
        : base(script){

      this.function = function;
    }

    public void StartCoroutine(T1 arg1, T2 arg2, T3 arg3){
      if(routine == null){
        routine = script.StartCoroutine(NullOnComplete(function(arg1, arg2, arg3)));
      }
    }
  }
}
