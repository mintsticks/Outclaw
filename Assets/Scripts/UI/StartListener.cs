using UnityEngine;
using UnityEngine.SceneManagement;

public class StartListener : MonoBehaviour {

  [SerializeField] private string nextScene = "Tutorial";

  void Update() {
    //TODO(daniel): replace all inputs to a global player input manager
    if (Input.GetKey(KeyCode.Space)) {
      SceneManager.LoadScene(nextScene);
    }
  }
}