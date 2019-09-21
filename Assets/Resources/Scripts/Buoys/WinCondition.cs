using UnityEngine;

public class WinCondition : MonoBehaviour {
    public void OnTriggerEnter2D(Collider2D collider) {
        if (collider.name.Equals("Queue Collider")) {
            Destroy(gameObject);
            Main.SharedInstance.Win();
        }
    }
}
