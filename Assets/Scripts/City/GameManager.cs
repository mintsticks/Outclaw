
using UnityEngine;

namespace Outclaw.City {
  public class GameManager {
    private static GameManager instance = new GameManager();
    private GameManager() {
      player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    
    private Player player;
    
    public static GameManager Instance {
      get { return instance ?? (instance = new GameManager()); }
    }

    public Player PlayerInstance {
      get {
        if (player == null) {
          player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }

        return player;
      }
    }
  }
}