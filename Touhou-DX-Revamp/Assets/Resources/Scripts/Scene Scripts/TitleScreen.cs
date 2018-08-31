using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour {
	void Update () {
        if (Input.GetKey(KeyCode.Return)) SceneManager.LoadScene("Main");
    }
}
