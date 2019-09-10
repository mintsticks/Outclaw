
using UnityEngine;

namespace Outclaw.City {
  public class GameManager {
    private static GameManager instance = new GameManager();
    private GameManager() {
      player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    
    private Player player;
    
    public static GameManager Instance => instance ?? (instance = new GameManager());
    public Player PlayerInstance => player;
  }
}