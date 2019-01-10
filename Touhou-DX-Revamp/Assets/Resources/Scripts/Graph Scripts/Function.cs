using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Vectrosity;

public class Function : MonoBehaviour {
    private Transform trans;
    private Equation eq;
    private FunctionProcess currProcess;
    private List<VectorLine> visualLines;
    private List<VectorLine> colliderLines;
    private List<List<Vector3>> points;

    public static Function Create(Transform trans, string prefab, Equation equation) {
        GameObject obj = Instantiate((GameObject)Resources.Load(prefab), InGameDimentions.center, Quaternion.identity, trans) as GameObject;
        Function func = obj.GetComponent<Function>();
        func.name = equation.name;
        func.eq = equation;
        return func;
    }
    void Awake() {
        trans = transform;
        gameObject.SetActive(false);
    }
    void OnEnable() {
        currProcess = FunctionProcess.UNDRAWN;
        reInitLines();
    }
    void Update(){
        if(CartesianPlane.SharedPlane.isGridShifting()) drawLines();
    }
    private void reInitLines() {
        points = new List<List<Vector3>>();
        visualLines = new List<VectorLine>();
        colliderLines = new List<VectorLine>();
    }
    private void destroyLines(){
        if (visualLines != null) VectorLine.Destroy(visualLines);
        if (colliderLines != null) VectorLine.Destroy(colliderLines);
    }
    private void drawLines() {
        foreach (VectorLine line in visualLines) line.Draw();
        foreach (VectorLine line in colliderLines) line.Draw();
    }
    public void disableFunction(){
        StopAllCoroutines();
        destroyLines();
        gameObject.SetActive(false);
    }
    public IEnumerator drawFunction(int subdivisions, float drawTime, float start, float end) {
        if(currProcess != FunctionProcess.UNDRAWN){
            Debug.Log("Drawing did not RUN");
            yield break;  //need a "did not run" message
        }
        currProcess = FunctionProcess.DRAWING;
        
        createNewSegment();
        int currLine = 0;

        int prevDiv = 0;
        int currDiv = 0;
        float prevInput = start;
        float currInput = start;
        for (float drawTimer = 0f; prevDiv <= subdivisions; drawTimer += Time.deltaTime){
            currDiv = Mathf.Min((int)(drawTimer / drawTime * subdivisions), subdivisions);
            for(; prevDiv <= currDiv; prevDiv++){
                currInput = start + (end - start) * prevDiv / subdivisions;
                
                //discontinuity handler
                Discontinuity disc = eq.checkDiscontinuity(prevInput, currInput);
                if(disc != null){
                    if(currInput == disc.param) points[currLine].Add(eq.getPoint(prevInput - (end - start) / subdivisions / 2));
                    else if(prevInput == disc.param) points[currLine].Add(eq.getPoint(prevInput + (end - start) / subdivisions / 2));
                    else points[currLine].Add(eq.getPoint(prevInput));

                    if(prevInput != disc.param){
                        if(disc.limitExistsAndIsNotInfinity(-1)){
                            if(disc.leftLimit == disc.trueValue) createAHole(disc.param, disc.leftLimit, true);
                            else createAHole(disc.param, disc.leftLimit, false);
                            currLine++;
                        }
                        if(disc.limitExistsAndIsNotInfinity(0) && disc.trueValue != disc.leftLimit){
                            createAHole(disc.param, disc.trueValue, true);
                            currLine++;
                        }
                        if(disc.limitExistsAndIsNotInfinity(1) && disc.rightLimit != disc.leftLimit && disc.rightLimit != disc.trueValue){
                            createAHole(disc.param, disc.rightLimit, false);
                            currLine++;
                        }
                    }

                    currLine++;
                    createNewSegment();
                }
                else if(prevInput != currInput) points[currLine].Add(eq.getPoint(prevInput));

                if (!colliderLines[currLine].collider && points[currLine].Count > 1) colliderLines[currLine].collider = true;
                prevInput = currInput;
            }
            drawLines();
            yield return null;
        }

        currProcess = FunctionProcess.IDLE;
    }
    private void createNewSegment() {
        points.Add(new List<Vector3>());

        VectorLine vLine = new VectorLine(eq.name + " VISUAL " + visualLines.Count, points[points.Count - 1], 10, LineType.Continuous, Joins.Fill);
        vLine.SetCanvas(CartesianPlane.SharedPlane.getFunctionCanvas(), false);
        vLine.color = Color.green;
        vLine.drawTransform = CartesianPlane.SharedPlane.transform;
        visualLines.Add(vLine);

        VectorLine cLine = new VectorLine(eq.name + " COLLIDER " + colliderLines.Count, points[points.Count - 1], 10, LineType.Continuous, Joins.Fill);
        cLine.SetCanvas(CartesianPlane.SharedPlane.getFunctionCanvas(), true);
        cLine.color = Color.clear;
        cLine.trigger = true;
        cLine.collider = false;
        cLine.rectTransform.gameObject.tag = "Function";
        cLine.drawTransform = CartesianPlane.SharedPlane.transform;
        colliderLines.Add(cLine);
    }
    private void createAHole(float param, float value, bool filled){
        createNewSegment();
        int count = points.Count - 1;

        Vector3 center = new Vector3(param, value);
       
        int segments = 30;
        for(int i = 1; i <= segments + 1; i++) points[count].Add(new Vector3());

        float radius = 0.1f;
        if(filled){
            radius = 0.05f;
            visualLines[count].SetWidth(20);
            colliderLines[count].SetWidth(20);
        }
        
        visualLines[count].MakeCircle(center, radius, segments);
        colliderLines[count].MakeCircle(center, radius, segments);
        colliderLines[count].collider = true;
    }

    public IEnumerator deleteFunction(float deleteTime) {  //deleting process
        if(currProcess != FunctionProcess.IDLE){
            Debug.Log("Deleting did not RUN");
            yield break;   //need a "did not run" message
        }
        currProcess = FunctionProcess.DELETING;

        //do stuff here
        destroyLines();
        reInitLines();
        yield return new WaitForSeconds(deleteTime);

        currProcess = FunctionProcess.UNDRAWN;
    }
    public FunctionProcess getCurrProcess(){
        return currProcess;
    }
}
public enum FunctionProcess {
    UNDRAWN,
    IDLE,
    DRAWING,
    DELETING,
    INTERMEDIATE_VAL,
    NEWTONS_METHOD,
    EULERS_METHOD,
    AVERGAE_VAL,
    LRAM,
    RRAM,
    MRAM,
    TRAPEZOID,
    SIMPSONS,
    INTEGERAL,
    MEAN_VAL,
    SLOPE_FIELD,
    WASHERS,
    SHELLS
}