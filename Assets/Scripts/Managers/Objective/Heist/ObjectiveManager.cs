using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Outclaw.Heist {
  public interface IObjectiveManager {
    void AddObjective(Objective objective);
    bool ObjectivesComplete();
  }
  
  public class ObjectiveManager : IObjectiveManager {
    private List<Objective> objectives = new List<Objective>();

    public void AddObjective(Objective objective) {
      objectives.Add(objective);
    }

    public bool ObjectivesComplete() {
      return objectives.All(obj => obj.IsComplete);
    }
  }
}