using UnityEngine;
using UnityEngine.SceneManagement;

public class StartListener : MonoBehaviour {
  void Update() {
    //TODO(daniel): replace all inputs to a global player input manager
    if (Input.GetKey(KeyCode.Space)) {
      SceneManager.LoadScene("Main");
    }
  }
}