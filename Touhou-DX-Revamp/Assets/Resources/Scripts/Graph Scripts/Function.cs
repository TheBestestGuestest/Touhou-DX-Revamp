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
    private int currDiv;
    private int subdivisions;
    private float drawTime;

    private string currProcess = null;
    private string rootProcess = null;

    private VectorLine visualLine;
    private VectorLine colliderLine;
    private List<Vector3> points;

    public static Function Create(Transform trans, string prefab, Equation equation, int subdiv, float drawTime, float start = 0, float end = 0) {
        GameObject obj = Instantiate((GameObject)Resources.Load(prefab), CartesianPlane.SharedPlane.getOrigin(), Quaternion.identity, trans) as GameObject;
        Function func = obj.GetComponent<Function>();
        func.name = equation.name;
        func.eq = equation;
        func.subdivisions = subdiv;
        func.drawTime = drawTime;
        func.start = start;
        func.end = end;
        return func;
    }

    void Awake() {
        trans = transform;
        gameObject.SetActive(false);
    }
    private void initLines() {
        if (visualLine == null && colliderLine == null) {
            points = new List<Vector3>();
            visualLine = new VectorLine(eq.name + " VISUAL", points, 10, LineType.Continuous, Joins.Fill);
            visualLine.SetCanvas(CartesianPlane.SharedPlane.getFunctionCanvas(), false);
            visualLine.color = Color.black;

            colliderLine = new VectorLine(eq.name + " COLLIDER", points, 10, LineType.Continuous, Joins.Fill);
            colliderLine.SetCanvas(CartesianPlane.SharedPlane.getFunctionCanvas(), true);
            colliderLine.color = Color.clear;
            colliderLine.trigger = true;
            colliderLine.collider = false;
            colliderLine.rectTransform.gameObject.tag = "Function";
        }
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
            for (int i = 0; i < points.Count; i++) {
                float input = start + (end - start) * i / subdivisions;
                points[i] = eq.posRelativeToPlane(input);
            }
            drawLines();
        }
    }

    public IEnumerator drawFunction() {
        currDiv = 0;

        for (float drawTimer = 0f; currDiv <= subdivisions; drawTimer += Time.deltaTime) {
            //spanning the grid if no start and end points are provided
            if (start == 0 && end == 0) {
                start = CartesianPlane.SharedPlane.getLeftEdge();
                end = CartesianPlane.SharedPlane.getRightEdge();  //MAKE IT SO FOR RECTS YOU SPAWN THE LINE THATS HIDDEN (SO WHEN YOU MOVE ITS NOT JEBAITED) but draw the visible parts
            }

            //makes up for the missing points
            float timeRatio = Mathf.Min(drawTimer / drawTime * subdivisions, subdivisions);

            for (; currDiv <= timeRatio; currDiv++) {
                float input = start + (end - start) * currDiv / subdivisions;
                currProcess = string.Concat(rootProcess, input);

                EquationList.SharedInstance.showEqList();
                points.Add(eq.posRelativeToPlane(input));
            }

            if (!colliderLine.collider && points.Count > 1) colliderLine.collider = true;

            drawLines();
            yield return null;
        }

        currProcess = "Inactive";
        EquationList.SharedInstance.showEqList();
        yield return new WaitForSeconds(3);
        StartCoroutine(doProcess());
    }

    public virtual IEnumerator doProcess() {
        while (points.Count > 0) {
            points.RemoveAt(0);

            if (points.Count <= 1 && colliderLine.collider) colliderLine.collider = false;

            drawLines();
            yield return null;
        }
        gameObject.SetActive(false);
    }

    private void drawLines() {
        visualLine.Draw();
        colliderLine.Draw();
    }

    public string getEqName() {
        return eq.name;
    }
    public string getCurrProcess() {
        return currProcess == null ? "Inactive" : currProcess;
    }
    public void destroyLines() {
        VectorLine.Destroy(ref visualLine);
        VectorLine.Destroy(ref colliderLine);
    }
}

public class Process{

}