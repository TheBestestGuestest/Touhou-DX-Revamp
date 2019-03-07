using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public GameObject GridNumbers;
    private TextMeshPro[] gridNums;
    private Canvas gridCanvas;
    private Material gridMaterial;
    private Texture gridTexture;

    void Awake() {
        SharedPlane = this;
        origin = transform;
        originLineIndex = (numGridLines - 1) / 2;
        setUpGridNums();
        gridNums = GridNumbers.GetComponents<TextMeshPro>();
        gridCanvas = GameObject.Find("Grid Canvas").GetComponent<Canvas>();
        gridMaterial = Instantiate(Resources.Load("Materials/Grid") as Material);
        gridTexture = Resources.Load("Textures/ThinLine") as Texture;
    }

    private void setUpGridNums(){
        Vector3 size = new Vector3(0.5f, 0.5f);
        Vector3 offset = new Vector3(-0.3f, -0.25f);
        int gridLayer = SortingLayer.NameToID("Grid");
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < numGridLines; j++) {
                int jIndex = j - originLineIndex;
                GameObject temp = new GameObject(i+" "+jIndex);
                TextMeshPro num = temp.AddComponent<TextMeshPro>();
                num.sortingLayerID = gridLayer;
                num.sortingOrder = -1;
                num.text = jIndex.ToString();
                num.alignment = TextAlignmentOptions.TopRight;
                num.fontSize = 2;
                num.rectTransform.sizeDelta = size;
                //num.font = sus;
                if(i == 0){
                    temp.transform.position = new Vector3(jIndex*xRatio(), 0) + offset;
                    float transparent = 1 - Mathf.Abs(Mathf.Round(gridProperties[0]) - gridProperties[0]);
                    num.color = Color.red * new Color(1, 1, 1, transparent);
                }
                else{ 
                    temp.transform.position = new Vector3(0, jIndex*yRatio()) + offset;
                    float transparent = 1 - Mathf.Abs(Mathf.Round(gridProperties[2]) - gridProperties[2]);
                    num.color = Color.red * new Color(1, 1, 1, transparent);
                }
                temp.transform.parent = GridNumbers.transform;
            }
        }
    }

    void Start() {
        for (int i = 0; i < 2; i++) {
            waitDelay[i] = new WaitUntil[5];
            for (int j = 0; j < waitDelay[i].Length; j++) waitDelay[i][j] = getWait(i, j);
        }

        origin.position = new Vector3(InGameDimentions.centerX, InGameDimentions.centerY, 3);
        gridLines = new VectorLine("GridLines", new List<Vector3>(), gridTexture, 5f, LineType.Discrete, Joins.None);
        gridLines.material = gridMaterial;
        gridLines.SetCanvas(gridCanvas, false);
        drawGrid();
        gridLines.SetColor(new Color(0f, 1f, 1f, 0.5f));
        gridLines.SetColor(Color.red, 0);
        gridLines.SetColor(Color.red, 1);
        gridLines.rectTransform.gameObject.tag = "Function";
        gridLines.layer = LayerMask.NameToLayer("PlayerEncounterable");
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

        //origin lines
        gridLines.points3.Add(origin.position + new Vector3(originLineIndex * xLength, 0));
        gridLines.points3.Add(origin.position + new Vector3(-originLineIndex * xLength, 0));
        gridLines.points3.Add(origin.position + new Vector3(0, originLineIndex * yLength));
        gridLines.points3.Add(origin.position + new Vector3(0, -originLineIndex * yLength));

        Vector3 point1;
        Vector3 point2;

        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < numGridLines; j++) {
                int jIndex = j - originLineIndex;
                if(jIndex == 0) continue;
                if (i == 0) {
                    point1 = origin.position + new Vector3(jIndex * xLength, originLineIndex * yLength);
                    point2 = origin.position + new Vector3(jIndex * xLength, -originLineIndex * yLength);
                }
                else {
                    point1 = origin.position + new Vector3(originLineIndex * xLength, jIndex * yLength);
                    point2 = origin.position + new Vector3(-originLineIndex * xLength, jIndex * yLength);
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
        if (origin.position.Equals(newOrigin)) yield break;
        //color pulse
        yield return pulseColorPoint(newOrigin);
        //shifting the var
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
        if (gridProperties[p] == newVal) yield break;
        //color pulse
        float oldVal = gridProperties[p];
        yield return pulseColorAxis(p < 2, p % 2 == 0, (p % 2 == 0) ? newVal > oldVal : newVal < oldVal);  //wowow messy
        //shifting the var
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
    public IEnumerator pulseColorAxis(bool horizontal, bool origin, bool inwards){  //sus
        yield return null;
    }
    public IEnumerator pulseColorPoint(Vector3 point){  //sus
        yield return null;
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
