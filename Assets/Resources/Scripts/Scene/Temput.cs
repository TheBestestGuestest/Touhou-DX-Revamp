using UnityEngine;
using UnityEngine.SceneManagement;

public class Temput : MonoBehaviour {
    void Update() {
        if (Input.GetKey(KeyCode.Escape)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
