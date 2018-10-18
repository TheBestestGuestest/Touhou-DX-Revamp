using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Vectrosity;

public class Function : MonoBehaviour {
    private Transform trans;
    private Equation eq;

    private float start;
    private float end;
    private int subdivisions;
    private float drawTime;

    private float levelOfdiscont;

    private string currProcess = null;
    private string rootProcess = null;

    private List<VectorLine> visualLines;
    private List<VectorLine> colliderLines;
    private List<List<Vector3>> points;

    public static Function Create(Transform trans, string prefab, Equation equation, int subdiv, float drawTime, float start, float end, float levelOfdiscont = float.PositiveInfinity) {
        GameObject obj = Instantiate((GameObject)Resources.Load(prefab), InGameDimentions.center, Quaternion.identity, trans) as GameObject;
        Function func = obj.GetComponent<Function>();
        func.name = equation.name;
        func.eq = equation;
        func.subdivisions = subdiv;
        func.drawTime = drawTime;
        func.start = start;
        func.end = end;
        func.levelOfdiscont = levelOfdiscont;
        return func;
    }

    void Awake() {
        trans = transform;
        gameObject.SetActive(false);
    }
    private void initLines() {
        points = new List<List<Vector3>>();
        visualLines = new List<VectorLine>();
        colliderLines = new List<VectorLine>();

        createNewSegment();
    }

    private void createNewSegment() {
        int prevLine = points.Count - 1;
        if (prevLine >= 0 && points[prevLine].Count == 1) {
            visualLines[prevLine].lineType = LineType.Points;
            colliderLines[prevLine].lineType = LineType.Points;
            //something about point hitboxes
        }

        points.Add(new List<Vector3>());

        VectorLine vLine = new VectorLine(eq.name + " VISUAL " + visualLines.Count, points[points.Count - 1], 10, LineType.Continuous, Joins.Fill);
        vLine.SetCanvas(CartesianPlane.SharedPlane.getFunctionCanvas(), false);
        vLine.color = Color.black;
        visualLines.Add(vLine);

        VectorLine cLine = new VectorLine(eq.name + " COLLIDER " + colliderLines.Count, points[points.Count - 1], 10, LineType.Continuous, Joins.Fill);
        cLine.SetCanvas(CartesianPlane.SharedPlane.getFunctionCanvas(), true);
        cLine.color = Color.clear;
        cLine.trigger = true;
        cLine.collider = false;
        cLine.rectTransform.gameObject.tag = "Function";
        colliderLines.Add(cLine);
    }

    void OnEnable() {
        initLines();

        //initializing equation
        switch (eq.type) {
            case EquationType.RECTANGULAR:
                rootProcess = "x = ";
                break;
            case EquationType.PARAMETRIC:
                rootProcess = "t = ";
                break;
            case EquationType.POLAR:
                rootProcess = "theta = ";
                break;
        }

        //drawing and processing
        EquationList.SharedInstance.addFunction(this);
        StartCoroutine(drawFunction());
    }

    void Update() {
        if (CartesianPlane.SharedPlane.isGridShifting()) {
            int currInput = 0;
            foreach (List<Vector3> segment in points) {
                for (int index = 0; index < segment.Count; index++) {
                    float eqinput = start + (end - start) * currInput / subdivisions;
                    segment[index] = eq.posRelativeToPlane(eqinput);

                    currInput++;
                }
            }
            drawLines();
        }
    }

    public IEnumerator drawFunction() {
        int currDiv = 0;
        Vector3 prevPoint = Vector3.positiveInfinity;
        int currLine = 0;

        for (float drawTimer = 0f; currDiv <= subdivisions; drawTimer += Time.deltaTime) {
            //makes up for the missing points
            float timeRatio = Mathf.Min(drawTimer / drawTime * subdivisions, subdivisions);
            for (; currDiv <= timeRatio; currDiv++) {
                float input = start + (end - start) * currDiv / subdivisions;
                Vector3 nextPoint = eq.posRelativeToPlane(input);

                //if the next point contains a NaN value, we ignore it as a discontinuity
                if (float.IsNaN(nextPoint.x) || float.IsNaN(nextPoint.y) || float.IsNaN(nextPoint.z)) {
                    prevPoint = Vector3.positiveInfinity;
                    currLine++;
                    createNewSegment();
                }
                else {
                    //if the next point is too far from the previous point, we start a new line segment
                    if (!prevPoint.Equals(Vector3.positiveInfinity) && Vector3.Distance(prevPoint, nextPoint) > levelOfdiscont) {
                        currLine++;
                        createNewSegment();
                    }
                    points[currLine].Add(nextPoint);
                    prevPoint = nextPoint;
                }

                currProcess = string.Concat(rootProcess, input);
                EquationList.SharedInstance.showEqList();
            }

            if (!colliderLines[currLine].collider && points[currLine].Count > 1) colliderLines[currLine].collider = true;

            drawLines();
            yield return null;
        }

        currProcess = "Inactive";
        EquationList.SharedInstance.showEqList();
        yield return new WaitForSeconds(2);
        StartCoroutine(doProcess());
    }

    public virtual IEnumerator doProcess() {  //DELETION SCREWS UP WHEN THE GRAPH IS CHANGING???
        int currDiv = 0;

        for (float drawTimer = 0f; currDiv <= subdivisions; drawTimer += Time.deltaTime) {
            //makes up for the missing points
            float timeRatio = Mathf.Min(drawTimer / drawTime * subdivisions, subdivisions);  //for now its drawtime lul
            for (; currDiv <= timeRatio; currDiv++) {
                if (points[0].Count > 0) {
                    points[0].RemoveAt(0);
                    if (colliderLines[0].collider && points[0].Count <= 1) colliderLines[0].collider = false;
                }
                else if (points.Count > 0) {
                    points.RemoveAt(0);
                }
            }
            drawLines();
            yield return null;
        }
        destroyLines();
        gameObject.SetActive(false);
    }

    private void drawLines() {
        foreach (VectorLine line in visualLines) line.Draw();
        foreach (VectorLine line in colliderLines) line.Draw();
    }
    public string getEqName() {
        return eq.name;
    }
    public string getCurrProcess() {
        return currProcess == null ? "Inactive" : currProcess;
    }
    public void destroyLines() {
        if (visualLines != null) VectorLine.Destroy(visualLines);
        if (colliderLines != null) VectorLine.Destroy(colliderLines);
    }
}

public enum Process {
    INT_VAL,
    NEWTON,
    EULER,
    AVE_VAL,
    LRAM,
    RRAM,
    MRAM,
    TRAPAZ,
    SIMPS,
    INTEG,
    MEAN_VAL,
    SLOPE_FIELD,
    WASHELLS
}
public class Processes {

}