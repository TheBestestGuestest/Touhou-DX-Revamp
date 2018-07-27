using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GridProperty {
    X_SCALE = 0,
    X_LENGTH = 1,
    Y_SCALE = 2,
    Y_LENGTH = 3
};

public class CartesianPlane : MonoBehaviour {
    public static CartesianPlane SharedPlane;

    private Transform origin;
    private float[] gridProperties = { 1f, 1f, 1f, 1f };  //ORDER: x-scale, x-length, y-scale, y-length
    private float xRatio() { return gridProperties[0] / gridProperties[1]; }
    private float yRatio() { return gridProperties[2] / gridProperties[3]; }

    private int[] numOpInQueue = new int[5];  //same order, #5 is origin
    private bool[] isOpRunning = new bool[5];  //same order, #5 is origin

    void Awake() {
        SharedPlane = this;
        origin = transform;
    }

    void Start() {
        origin.position = new Vector3(InGameDimentions.centerX, InGameDimentions.centerY, 0);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha0)) StartCoroutine(shiftOrigin(new Vector3(-1f, Random.value), 1f));
        if (Input.GetKeyDown(KeyCode.Alpha1)) StartCoroutine(setProperty(GridProperty.X_SCALE, 2f, 1f));
        if (Input.GetKeyDown(KeyCode.Alpha2)) StartCoroutine(setProperty(GridProperty.X_LENGTH, 2f, 1f));
        if (Input.GetKeyDown(KeyCode.Alpha3)) StartCoroutine(setProperty(GridProperty.Y_SCALE, 2f, 1f));
        if (Input.GetKeyDown(KeyCode.Alpha4)) StartCoroutine(setProperty(GridProperty.Y_LENGTH, 2f, 1f));

        int numLines = 10;
        float xLength = gridProperties[(int)GridProperty.X_LENGTH];
        float yLength = gridProperties[(int)GridProperty.Y_LENGTH];
        for (int i = -numLines; i <= numLines; i++)
            Debug.DrawLine(origin.position + new Vector3(i * xLength, numLines * yLength), origin.position + new Vector3(i * xLength, -numLines * yLength), i == 0 ? Color.red : Color.blue);
        for (int i = -numLines; i <= numLines; i++)
            Debug.DrawLine(origin.position + new Vector3(numLines * xLength, i * yLength), origin.position + new Vector3(-numLines * xLength, i * yLength), i == 0 ? Color.red : Color.blue);
    }

    public IEnumerator shiftOrigin(Vector3 newOrigin, float time) {
        int p = 4;

        //queueing stuff
        int queue = ++numOpInQueue[p];
        while (queue > 0) {
            yield return new WaitUntil(() => !isOpRunning[p]);
            queue--;
        }
        numOpInQueue[p]--;
        isOpRunning[p] = true;

        //shifting the var
        if (origin.position.Equals(newOrigin)) yield break;
        newOrigin.z = origin.position.z;
        Vector3 oldOrigin = origin.position;
        float timeElapsed = 0f;
        while (timeElapsed + Time.deltaTime <= time) {
            timeElapsed += Time.deltaTime;
            float newPosX = oldOrigin.x + (newOrigin.x - oldOrigin.x) * timeElapsed / time;
            float newPosY = oldOrigin.y + (newOrigin.y - oldOrigin.y) * timeElapsed / time;
            origin.position = new Vector3(newPosX, newPosY, newOrigin.z);
            yield return null;
        }
        origin.position = newOrigin;

        isOpRunning[p] = false;  //unqueue the command
    }
    public IEnumerator setProperty(GridProperty gp, float newVal, float time) {
        int p = (int)gp;

        //queueing stuff
        int queue = ++numOpInQueue[p];
        while (queue > 0) {
            yield return new WaitUntil(() => !isOpRunning[p]);
            queue--;
        }
        numOpInQueue[p]--;
        isOpRunning[p] = true;

        //shifting the var
        if (gridProperties[p] == newVal) yield break;

        float oldVal = gridProperties[p];
        float timeElapsed = 0f;
        while (timeElapsed + Time.deltaTime <= time) {
            timeElapsed += Time.deltaTime;
            gridProperties[p] = oldVal + (newVal - oldVal) * timeElapsed / time;
            yield return null;
        }
        gridProperties[p] = newVal;

        isOpRunning[p] = false;  //unqueue the command
    }

    public bool isGridShifting() {
        for (int i = 0; i < isOpRunning.Length; i++) if (isOpRunning[i]) return true;
        return false;
    }
    public Vector3 pointRelativeToOrigin(Vector3 point) {
        return new Vector3(origin.position.x + (point.x * xRatio()), origin.position.y + (point.y * yRatio()), origin.position.z);
    }
    public float getDrawInterval() {
        return 0.04f * xRatio();
    }
    public float getLeftEdge() {
        return (InGameDimentions.leftEdge - origin.position.x) / xRatio();
    }
    public float getRightEdge() {
        return (InGameDimentions.rightEdge - origin.position.x) / xRatio();
    }
    public float getTopEdge() {
        return (InGameDimentions.topEdge - origin.position.y) / yRatio();
    }
    public float getBottomEdge() {
        return (InGameDimentions.bottomEdge - origin.position.y) / yRatio();
    }
}

public class InGameDimentions {
    public static float rightEdge = 3f;
    public static float leftEdge = -20.0f / 3.0f;
    public static float topEdge = 5f;
    public static float bottomEdge = -5f;

    public static float screenWidth = rightEdge - leftEdge;
    public static float screenHeight = topEdge - bottomEdge;

    public static float centerX = (rightEdge + leftEdge) / 2.0f;
    public static float centerY = (topEdge + bottomEdge) / 2.0f;
}
