using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToTitle : MonoBehaviour {
    void Update() {
        if (Input.GetKey(KeyCode.Return)) SceneManager.LoadScene("StartScreen");
    }
}
