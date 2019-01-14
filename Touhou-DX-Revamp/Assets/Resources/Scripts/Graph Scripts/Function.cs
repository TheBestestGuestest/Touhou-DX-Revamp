using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Vectrosity;

public class Function : MonoBehaviour {
    public static float currXscale = 1;
    public static float currYscale = 1;
    public static Vector3 currOffset = new Vector3(0,0);
    private Transform trans;
    private Equation eq;
    private FunctionProcess currProcess;
    private FunctionLine line;

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
        line = new FunctionLine();
    }
    void Update(){
        if(currProcess == FunctionProcess.IDLE &&
        (currXscale != CartesianPlane.SharedPlane.xRatio()
        || currYscale != CartesianPlane.SharedPlane.yRatio()
        || !currOffset.Equals(CartesianPlane.SharedPlane.getOrigin()))) {
            currXscale = CartesianPlane.SharedPlane.xRatio();
            currYscale = CartesianPlane.SharedPlane.yRatio();
            currOffset = CartesianPlane.SharedPlane.getOrigin();
            line.drawLines();
        }
    }
    public void disableFunction(){
        StopAllCoroutines();
        line.destroyLines();
        gameObject.SetActive(false);
    }
    public IEnumerator drawFunction(int subdivisions, float drawTime, float start, float end) {  //sus
        if(currProcess != FunctionProcess.UNDRAWN){
            Debug.Log("Drawing did not RUN");
            yield break;
        }
        currProcess = FunctionProcess.DRAWING;

        line.createLine();
        float interval = (end - start) / subdivisions;
        int prevDiv = 0;
        int currDiv = 0;
        for (float drawTimer = 0f; prevDiv <= subdivisions; drawTimer += Time.deltaTime){
            currDiv = Mathf.Min((int)(drawTimer / drawTime * subdivisions), subdivisions);
            for(; prevDiv <= currDiv; prevDiv++){
                float currInput = start + prevDiv * interval;
                float frontInput = currInput + interval;

                Discontinuity d = eq.checkDiscontinuity(currInput, frontInput);
                if(d != null && d.param != frontInput){
                    if(currInput != d.param) line.originalPoints.Last().Add(eq.getPoint(currInput));
                    for(int i = 0; i < d.holes.Count; i++) line.createHole(d.holes[i], d.isFilled[i]);
                    line.createLine();
                }
                else line.originalPoints.Last().Add(eq.getPoint(currInput));
            }
            line.drawLines();
            yield return null;
        }

        currProcess = FunctionProcess.IDLE;
    }
    public IEnumerator deleteFunction(float deleteTime) {  //sus deleting process
        if(currProcess != FunctionProcess.IDLE){
            Debug.Log("Deleting did not RUN");
            yield break;
        }
        currProcess = FunctionProcess.DELETING;

        //do stuff here
        line.destroyLines();
        line = new FunctionLine();
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

public class FunctionLine{
    public List<List<Vector3>> originalPoints;
    public List<bool> isHole;
    public List<bool> isFilled;
    public List<VectorLine> lines;
    public FunctionLine(){
        originalPoints = new List<List<Vector3>>();
        lines = new List<VectorLine>();
        isHole = new List<bool>();
        isFilled = new List<bool>();
    }
    public void createLine(){
        originalPoints.Add(new List<Vector3>());
        isHole.Add(false);
        isFilled.Add(false);
        VectorLine line = new VectorLine(""+originalPoints.Count, new List<Vector3>(), 10, LineType.Continuous, Joins.Fill);
        line.SetCanvas(CartesianPlane.SharedPlane.getFunctionCanvas());
        line.color = Color.green;
        line.trigger = true;
        line.collider = false;
        line.rectTransform.gameObject.tag = "Function";
        lines.Add(line);
    }
    public void createHole(Vector3 hole, bool filled){
        createLine();
        int index = originalPoints.Count - 1;
        originalPoints[index].Add(hole);
        isHole[index] = true;
        isFilled[index] = filled;
    }
    public void drawLines(){
        for(int i = 0; i < originalPoints.Count; i++){
            List<Vector3> temp = new List<Vector3>();
            if(isHole[i]){
                Vector3 center = CartesianPlane.SharedPlane.pointRelativeToOrigin(originalPoints[i][0]);
                int segments = 30;
                for(int j = 1; j <= segments + 1; j++) temp.Add(new Vector3());

                float radius = 0.1f;
                float width = 10;
                if(isFilled[i]){  //sus
                    radius /= 2;
                    lines[i].SetWidth(20);
                }
                lines[i].points3 = temp;
                lines[i].SetWidth(width);
                lines[i].MakeCircle(center, radius, segments);
                lines[i].color = Color.clear;
            }
            else{
                foreach(Vector3 point in originalPoints[i]) temp.Add(CartesianPlane.SharedPlane.pointRelativeToOrigin(point));
                lines[i].points3 = temp;
            }
            if(temp.Count > 1) lines[i].collider = true;
        }

        foreach(VectorLine line in lines) line.Draw();
    }
    public void destroyLines(){
        VectorLine.Destroy(lines);
    }
}