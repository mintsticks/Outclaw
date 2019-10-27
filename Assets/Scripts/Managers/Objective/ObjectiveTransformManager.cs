using System.Collections.Generic;
using System.Linq;
using Outclaw;
using Outclaw.City;
using UnityEngine;
using Zenject;

namespace City {

  public interface IObjectiveTransformManager {
    void RegisterTask(IHaveTask task);
    Transform GetTransformOfTask(Task task);
  }
  
  public class ObjectiveTransformManager : MonoBehaviour, IObjectiveTransformManager {

    private List<IHaveTask> tasks = new List<IHaveTask>();

    public void RegisterTask(IHaveTask task) => tasks.Add(task);

    public Transform GetTransformOfTask(Task task){
      return tasks.FirstOrDefault(i => i.ContainedTask == task)?.Location;
    }
  }
}