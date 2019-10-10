using System.Collections.Generic;
using System.Linq;
using Outclaw;
using Outclaw.City;
using UnityEngine;
using Zenject;

namespace City {

  public interface IObjectiveTransformManager {
    Transform GetTransformOfObject(ObjectType type);
    Transform GetTransformOfCat(CatType type);
    Transform GetTransformOfEntrance(EntranceType type);
    List<InteractableObject> Objects { get; }
    List<InteractableCat> Cats { get; }
    List<InteractableLocation> Locations { get; }
    //Use different location type thing?
  }
  
  public class ObjectiveTransformManager : MonoBehaviour, IObjectiveTransformManager {
    private List<InteractableObject> objects;
    private List<InteractableCat> cats;
    private List<InteractableLocation> locations;

    public List<InteractableObject> Objects => objects;
    public List<InteractableCat> Cats => cats;
    public List<InteractableLocation> Locations => locations;

    public void Awake() {
      objects = new List<InteractableObject>();
      cats = new List<InteractableCat>();
      locations = new List<InteractableLocation>();
    }
    
    public Transform GetTransformOfObject(ObjectType type) {
      return objects.FirstOrDefault(i => i.ObjectType == type)?.transform;
    }

    public Transform GetTransformOfCat(CatType type) {
      return cats.FirstOrDefault(i => i.CatType == type)?.transform;
    }
    
    public Transform GetTransformOfEntrance(EntranceType type) {
      return locations.FirstOrDefault(i => i.Type == type)?.transform;
    }
  }
}