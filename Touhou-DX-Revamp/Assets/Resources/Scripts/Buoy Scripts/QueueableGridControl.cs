using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueableGridControl : MonoBehaviour {
    public GridProperty selectedProperty;
    public Vector3 newOrigin;
    public float newValue;
    public float time;
    private string queueName = "Queue Collider";

    public void OnTriggerEnter2D(Collider2D collider) {
        if (collider.name.Equals(queueName)) {
            if (selectedProperty == GridProperty.ORIGIN && newOrigin != null) CartesianPlane.SharedPlane.shiftOrigin(newOrigin, time);
            else CartesianPlane.SharedPlane.setProperty(selectedProperty, newValue, time);
            Destroy(gameObject);
        }
    }
}
