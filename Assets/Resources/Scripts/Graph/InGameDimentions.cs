using UnityEngine;

public class InGameDimentions {
    public static float rightEdge = 3f;
    public static float leftEdge = -20.0f / 3.0f;
    public static float topEdge = 5f;
    public static float bottomEdge = -5f;

    public static float screenWidth = rightEdge - leftEdge;
    public static float screenHeight = topEdge - bottomEdge;

    public static float centerX = (rightEdge + leftEdge) / 2.0f;
    public static float centerY = (topEdge + bottomEdge) / 2.0f;

    public static Vector3 center = new Vector3(centerX, centerY, 0);
}