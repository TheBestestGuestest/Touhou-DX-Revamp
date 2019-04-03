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
    private int numGridLines = 31;
    private int originLineIndex;
    private float[] gridProperties = { 1f, 1f, 1f, 1f };  //ORDER: x-scale, x-length, y-scale, y-length
    public float xRatio() { return gridProperties[0] / gridProperties[1]; }
    public float yRatio() { return gridProperties[2] / gridProperties[3]; }

    private int[] numOpInQueue = new int[5];  //same order, #5 is origin
    private bool[] isOpRunning = new bool[5];  //same order, #5 is origin
    private WaitUntil[][] waitDelay = new WaitUntil[2][];  //dont worry about why there 2 just know that IT WORKS

    private VectorLine gridLines;
    private List<int> gridLineOrder;  //order in which the grid lines are drawn in
    private List<Color32> gridLineColors;  //color of each grid line (order independent from gridLine order)
    private bool frontOriginNum; //false = horizontal, true = vertical
    private Canvas gridCanvas;
    private Material gridMaterial;
    private Texture gridTexture;
    public GameObject GridNumbersParent;
    private GameObject[][] gridNums;
    private Vector3 gridNumsOffset = new Vector3(-0.3f, -0.3f);
    private Color originColor = Color.red;
    private Color baseColor = new Color(0, 1, 1, 0.5f);


    void Awake() {
        SharedPlane = this;
        origin = transform;
        originLineIndex = (numGridLines - 1) / 2;

        gridCanvas = GameObject.Find("Grid Canvas").GetComponent<Canvas>();
        gridMaterial = Instantiate(Resources.Load("Materials/Grid") as Material);
        gridTexture = Resources.Load("Textures/ThinLine") as Texture;
    }
    private WaitUntil getWait(int i, int j) {
        if (i == 0) return new WaitUntil(() => !isOpRunning[j]);
        else return new WaitUntil(() => isOpRunning[j]);
    }
    void Start() {
        for (int i = 0; i < 2; i++) {
            waitDelay[i] = new WaitUntil[5];
            for (int j = 0; j < waitDelay[i].Length; j++) waitDelay[i][j] = getWait(i, j);
        }
        gridLineOrder = new List<int>();
        gridLineColors = new List<Color32>();
        for (int i = 0; i < numGridLines * 2; i++) {
            if (i % numGridLines == originLineIndex) {
                gridLineOrder.Insert(0, i);
                gridLineColors.Add(originColor);
            }
            else {
                gridLineOrder.Add(i);
                gridLineColors.Add(baseColor);
            }
        }
        setUpGridNums();

        origin.position = new Vector3(InGameDimentions.centerX, InGameDimentions.centerY, 3);
        gridLines = new VectorLine("GridLines", new List<Vector3>(), gridTexture, 5f, LineType.Discrete, Joins.None);
        gridLines.material = gridMaterial;
        gridLines.SetCanvas(gridCanvas, false);
        drawGrid();
        gridLines.rectTransform.gameObject.tag = "Function";
        gridLines.layer = LayerMask.NameToLayer("PlayerEncounterable");
    }
    private void setUpGridNums() {
        gridNums = new GameObject[2][];
        Vector3 size = new Vector3(0.5f, 0.5f);
        int gridLayer = SortingLayer.NameToID("Grid");
        TMP_FontAsset gridFont = Resources.Load("Fonts/RoundPixels SDF") as TMP_FontAsset;
        for (int i = 0; i < 2; i++) {
            gridNums[i] = new GameObject[numGridLines];
            for (int j = 0; j < numGridLines; j++) {
                int jIndex = j - originLineIndex;
                GameObject temp = new GameObject(i + " " + jIndex);
                TextMeshPro num = temp.AddComponent<TextMeshPro>();
                num.sortingLayerID = gridLayer;
                num.sortingOrder = -1;
                num.text = jIndex.ToString();
                num.color = Color.red;
                num.alignment = TextAlignmentOptions.TopRight;
                num.fontSize = 2;
                num.rectTransform.sizeDelta = size;
                num.font = gridFont;
                temp.transform.SetParent(GridNumbersParent.transform);
                gridNums[i][j] = temp;
            }
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha0)) StartCoroutine(shiftOrigin(InGameDimentions.center + new Vector3(Random.value * 2f, Random.value * 2f), 1f));
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            float rand = (int)(1 + Random.value * 3);
            if (Random.value >= 0.5f) rand = 1 / rand;
            StartCoroutine(setProperty(GridProperty.X_SCALE, rand, 1f));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) StartCoroutine(setProperty(GridProperty.X_LENGTH, 0.5f + Random.value, 1f));
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            float rand = (int)(1 + Random.value * 3);
            if (Random.value >= 0.5f) rand = 1 / rand;
            StartCoroutine(setProperty(GridProperty.Y_SCALE, rand, 1f));
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) StartCoroutine(setProperty(GridProperty.Y_LENGTH, 0.5f + Random.value, 1f));
    }
    public void drawGrid() {
        gridLines.points3.Clear();
        float xLength = gridProperties[(int)GridProperty.X_LENGTH];
        float yLength = gridProperties[(int)GridProperty.Y_LENGTH];
        Vector3 point1;
        Vector3 point2;
        for (int i = 0; i < numGridLines * 2; i++) {
            int lineIndex = gridLineOrder[i];
            int jIndex;
            if (lineIndex < numGridLines) {
                jIndex = originLineIndex - lineIndex;
                point1 = origin.position + new Vector3(jIndex * xLength, originLineIndex * yLength);
                point2 = origin.position + new Vector3(jIndex * xLength, -originLineIndex * yLength);
            }
            else {
                jIndex = originLineIndex - (lineIndex - numGridLines);
                point1 = origin.position + new Vector3(originLineIndex * xLength, jIndex * yLength);
                point2 = origin.position + new Vector3(-originLineIndex * xLength, jIndex * yLength);
            }
            gridLines.points3.Add(point1);
            gridLines.points3.Add(point2);
        }
        colorGridLines();
        gridLines.Draw();
        drawGridNums();
    }
    private void colorGridLines() {
        for (int i = 0; i < numGridLines * 2; i++) gridLines.SetColor(gridLineColors[gridLineOrder[i]], i);
    }
    private void drawGridNums() {
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < numGridLines; j++) {
                int jIndex = j - originLineIndex;
                GameObject temp = gridNums[i][j];
                TextMeshPro num = temp.GetComponent<TextMeshPro>();

                float ratioPosition = jIndex / (i == 0 ? xRatio() : yRatio());
                float scale = i == 0 ? gridProperties[0] : gridProperties[2];
                float length = i == 0 ? gridProperties[1] : gridProperties[3];
                temp.transform.position = new Vector3(ratioPosition * (1 - i % 2), ratioPosition * (i % 2)) + gridNumsOffset + origin.position;

                Color numColor = num.color;
                float transparency = Mathf.Abs(Mathf.Round(ratioPosition / length) * length - ratioPosition) / length * 2f;
                if (scale > 1) transparency *= scale;
                transparency = Mathf.Max(1 - 1.5f * transparency * transparency, 0);
                numColor.a = transparency;
                num.color = jIndex == 0 && ((frontOriginNum && i != 0) || (!frontOriginNum && i == 0)) ? Color.clear : numColor;
            }
        }
    }

    public IEnumerator shiftOrigin(Vector3 newOrigin, float time) {
        //queueing stuff
        int p = 4;
        yield return queueCommand(p);
        if (origin.position.Equals(newOrigin)) {
            yield return null;
            isOpRunning[p] = false;
            yield break;
        }
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
        yield return queueCommand(p);
        if (gridProperties[p] == newVal) {
            yield return null;
            isOpRunning[p] = false;
            yield break;
        }
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
        colorReset(gp);
        isOpRunning[p] = false;
    }
    private IEnumerator queueCommand(int p) {
        int queue = ++numOpInQueue[p];
        while (queue > 0) {
            yield return waitDelay[0][p];
            queue--;
            if (queue > 0) yield return waitDelay[1][p];
        }
        numOpInQueue[p]--;
        isOpRunning[p] = true;
    }
    public IEnumerator pulseColorAxis(bool horizontal, bool nums, bool inwards) {  //sus ugly probbly should just split into two tbh
        Color pulsedColor = new Color(1, 1, 0);
        Color dimmedColor = new Color(0.9f, 0.9f, 0);
        WaitForSeconds delay = new WaitForSeconds(0.05f);
        int i = horizontal ? 0 : 1;
        for (int j = 0; j <= originLineIndex; j++) {
            int jIndex = inwards ? j : originLineIndex - j;
            if (nums) {
                TextMeshPro numLeft = gridNums[i][jIndex].GetComponent<TextMeshPro>();
                TextMeshPro numRight = gridNums[i][numGridLines - 1 - jIndex].GetComponent<TextMeshPro>();
                numLeft.color = new Color(pulsedColor.r, pulsedColor.g, pulsedColor.b, numLeft.color.a);
                numRight.color = new Color(pulsedColor.r, pulsedColor.g, pulsedColor.b, numRight.color.a);
                if (jIndex == originLineIndex) {
                    frontOriginNum = horizontal;
                    drawGridNums();
                }
                yield return delay;
                numLeft.color = new Color(dimmedColor.r, dimmedColor.g, dimmedColor.b, numLeft.color.a);
                numRight.color = new Color(dimmedColor.r, dimmedColor.g, dimmedColor.b, numRight.color.a);
                if (jIndex == originLineIndex) {
                    frontOriginNum = horizontal;
                    drawGridNums();
                }
            }
            else {
                int line1 = i * numGridLines + jIndex;
                int line2 = i * numGridLines + (numGridLines - 1 - jIndex);
                int top = line1 == line2 ? 0 : 2;
                float transparency = line1 == line2 ? originColor.a : baseColor.a;

                gridLineColors[line1] = pulsedColor;
                gridLineColors[line2] = pulsedColor;
                if (gridLineOrder.Remove(line1)) gridLineOrder.Insert(top, line1);
                if (gridLineOrder.Remove(line2)) gridLineOrder.Insert(top, line2);
                drawGrid();
                yield return delay;
                gridLineColors[line1] = new Color(dimmedColor.r, dimmedColor.g, dimmedColor.b, transparency);
                gridLineColors[line2] = new Color(dimmedColor.r, dimmedColor.g, dimmedColor.b, transparency);
                if (gridLineOrder.Remove(line1)) gridLineOrder.Insert(top, line1);
                if (gridLineOrder.Remove(line2)) gridLineOrder.Insert(top, line2);
            }
        }
        yield return null;
    }
    public IEnumerator pulseColorPoint(Vector3 point) {  //sus
        yield return null;
    }
    private void colorReset(GridProperty gp) {
        int axis = (int)gp / 2;
        switch (gp) {
            case GridProperty.X_LENGTH:
            case GridProperty.Y_LENGTH:
                for (int i = numGridLines * axis; i < numGridLines * (axis + 1); i++)
                    gridLineColors[i] = i % numGridLines == originLineIndex ? originColor : baseColor;
                colorGridLines();
                break;
            case GridProperty.X_SCALE:
            case GridProperty.Y_SCALE:
                for (int i = 0; i < numGridLines; i++) {
                    TextMeshPro num = gridNums[axis][i].GetComponent<TextMeshPro>();
                    num.color = new Color(originColor.r, originColor.g, originColor.b, num.color.a);
                }
                break;
        }
    }
    public Vector3 pointRelativeToOrigin(Vector3 point) {
        return new Vector3(origin.position.x + (point.x / xRatio()), origin.position.y + (point.y / yRatio()), origin.position.z);  //sus
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
