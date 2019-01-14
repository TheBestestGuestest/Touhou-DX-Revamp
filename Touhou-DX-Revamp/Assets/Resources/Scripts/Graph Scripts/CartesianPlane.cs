using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public enum GridProperty {
    X_SCALE = 0,
    X_LENGTH = 1,
    Y_SCALE = 2,
    Y_LENGTH = 3,
    ORIGIN = 4
};

public class CartesianPlane : MonoBehaviour {
    public static CartesianPlane SharedPlane;

    private Transform origin;
    private float[] gridProperties = { 1f, 1f, 1f, 1f };  //ORDER: x-scale, x-length, y-scale, y-length
    public float xRatio() { return gridProperties[0] / gridProperties[1]; }
    public float yRatio() { return gridProperties[2] / gridProperties[3]; }

    private int[] numOpInQueue = new int[5];  //same order, #5 is origin
    private bool[] isOpRunning = new bool[5];  //same order, #5 is origin
    private WaitUntil[][] waitDelay = new WaitUntil[2][];  //dont worry about why there 2 just know that IT WORKS

    private int numGridLines = 31;
    private int originLineIndex;
    private VectorLine gridLines;
    private Canvas gridCanvas;
    private Canvas functionCanvas;
    public Canvas getFunctionCanvas() { return functionCanvas; }

    void Awake() {
        SharedPlane = this;
        origin = transform;
        gridCanvas = GameObject.Find("Grid Canvas").GetComponent<Canvas>();
        functionCanvas = GameObject.Find("Function Canvas").GetComponent<Canvas>();
        originLineIndex = (numGridLines - 1) / 2;
    }

    void Start() {
        for (int i = 0; i < 2; i++) {
            waitDelay[i] = new WaitUntil[5];
            for (int j = 0; j < waitDelay[i].Length; j++) waitDelay[i][j] = getWait(i, j);
        }

        origin.position = new Vector3(InGameDimentions.centerX, InGameDimentions.centerY, 3);
        gridLines = new VectorLine("GridLines", new List<Vector3>(), 4f, LineType.Discrete, Joins.None);
        gridLines.SetCanvas(gridCanvas, false);
        
        drawGrid();
        gridLines.SetColor(new Color(0f, 1f, 1f, 0.5f));
        gridLines.SetColor(Color.red, originLineIndex);
        gridLines.SetColor(Color.red, originLineIndex + numGridLines);
    }
    private WaitUntil getWait(int i, int j) {
        if (i == 0) return new WaitUntil(() => !isOpRunning[j]);
        else return new WaitUntil(() => isOpRunning[j]);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha0)) StartCoroutine(shiftOrigin(InGameDimentions.center + new Vector3(Random.value * 2f, Random.value * 2f), 2f));
        if (Input.GetKeyDown(KeyCode.Alpha1)) StartCoroutine(setProperty(GridProperty.X_SCALE, 1f + Random.value, 1f));
        if (Input.GetKeyDown(KeyCode.Alpha2)) StartCoroutine(setProperty(GridProperty.X_LENGTH, 0.5f + Random.value, 1f));
        if (Input.GetKeyDown(KeyCode.Alpha3)) StartCoroutine(setProperty(GridProperty.Y_SCALE, 1f + Random.value, 1f));
        if (Input.GetKeyDown(KeyCode.Alpha4)) StartCoroutine(setProperty(GridProperty.Y_LENGTH, 0.5f + Random.value, 1f));
    }
    public void drawGrid() {
        gridLines.points3.Clear();
        float xLength = gridProperties[(int)GridProperty.X_LENGTH];
        float yLength = gridProperties[(int)GridProperty.Y_LENGTH];
        Vector3 point1;
        Vector3 point2;

        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < numGridLines; j++) {
                int jIndex = j - originLineIndex;
                float z = j == 0 ? origin.position.z + 1 : origin.position.z;
                if (i == 0) {
                    point1 = origin.position + new Vector3(jIndex * xLength, originLineIndex * yLength, z);
                    point2 = origin.position + new Vector3(jIndex * xLength, -originLineIndex * yLength, z);
                }
                else {
                    point1 = origin.position + new Vector3(originLineIndex * xLength, jIndex * yLength, z);
                    point2 = origin.position + new Vector3(-originLineIndex * xLength, jIndex * yLength, z);
                }
                gridLines.points3.Add(point1);
                gridLines.points3.Add(point2);
            }
        }
        gridLines.Draw();
    }

    public IEnumerator shiftOrigin(Vector3 newOrigin, float time) {
        //queueing stuff
        int p = 4;
        int queue = ++numOpInQueue[p];
        while (queue > 0) {
            yield return waitDelay[0][p];
            queue--;
            if (queue > 0) yield return waitDelay[1][p];
        }
        isOpRunning[p] = true;
        numOpInQueue[p]--;
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
            drawGrid();
            yield return null;
        }
        origin.position = newOrigin;
        drawGrid();
        //unqueue the command
        isOpRunning[p] = false;
    }
    public IEnumerator setProperty(GridProperty gp, float newVal, float time) {
        //queueing stuff
        int p = (int)gp;
        int queue = ++numOpInQueue[p];
        while (queue > 0) {
            yield return waitDelay[0][p];
            queue--;
            if (queue > 0) yield return waitDelay[1][p];
        }
        isOpRunning[p] = true;
        numOpInQueue[p]--;
        //shifting the var
        if (gridProperties[p] == newVal) yield break;
        float oldVal = gridProperties[p];
        float timeElapsed = 0f;
        while (timeElapsed + Time.deltaTime <= time) {
            timeElapsed += Time.deltaTime;
            gridProperties[p] = oldVal + (newVal - oldVal) * timeElapsed / time;
            drawGrid();
            yield return null;
        }
        gridProperties[p] = newVal;
        drawGrid();
        //unqueue the command
        isOpRunning[p] = false;
    }
    public Vector3 pointRelativeToOrigin(Vector3 point) {
        return new Vector3(origin.position.x + (point.x * xRatio()), origin.position.y + (point.y * yRatio()), origin.position.z);
    }
    public Vector3 getOrigin() {
        return origin.position;
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

    public static Vector3 center = new Vector3(centerX, centerY, 0);
}
