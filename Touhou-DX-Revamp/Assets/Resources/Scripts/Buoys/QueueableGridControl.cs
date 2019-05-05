using UnityEngine;

public class QueueableGridControl : MonoBehaviour {
    public GridProperty selectedProperty;
    public Vector3 newOrigin;
    public float newValue;
    public float time;

    public void OnTriggerEnter2D(Collider2D collider) {
        if (collider.name.Equals("Queue Collider")) {
            if (selectedProperty == GridProperty.ORIGIN) CartesianPlane.SharedPlane.shiftOrigin(newOrigin, time);
            else CartesianPlane.SharedPlane.setProperty(selectedProperty, newValue, time);
            Destroy(gameObject);
        }
    }
}
