using System;
using UnityEngine;

namespace Outclaw {
  /// <summary>
  /// Class to hold all data stored for the player.
  /// </summary>
  public interface IPlayerData {
    /// <summary>
    /// Name that the player has chosen for their cat.
    /// </summary>
    string Name { get; set; }
    
    //TODO(dwong): for now, represents number of jumps. replace with actual cat data
    int CatCount { get; set; }

    /// <summary>
    /// Save instance data to the system.
    /// </summary>
    void Save();
    
    /// <summary>
    /// Load data stored in system to instance.
    /// </summary>
    void Load();
  }

  [Serializable]
  public class PlayerData : IPlayerData {
    private static string SAVE_DATA_KEY = "GAMEDATA";
    
    [SerializeField]
    private string name = "Kuro";

    [SerializeField]
    private int catCount;
    
    public string Name {
      get { return name; }
      set { name = value; }
    }

    public int CatCount {
      get { return catCount; }
      set { catCount = value; }
    }

    public void Save() {
      var jsonData = JsonUtility.ToJson(this);
      PlayerPrefs.SetString(SAVE_DATA_KEY, jsonData);
    }

    public void Load() {
      string jsonData = PlayerPrefs.GetString(SAVE_DATA_KEY, string.Empty);
      if (jsonData == string.Empty) {
        return;
      }

      var data = JsonUtility.FromJson<PlayerData>(jsonData);
      LoadFromPlayerData(data);
    }

    private void LoadFromPlayerData(PlayerData data) {
      name = data.name;
      catCount = data.catCount;
    }
  }
}