using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Vectrosity;

public enum FunctionType{ DISCONTINUOUS, CONTINUOUS}
public class Function : MonoBehaviour {
    public static float currXscale = 1;
    public static float currYscale = 1;
    public static Vector3 currOffset = new Vector3(0,0);
    private Transform trans;
    private Equation eq;
    private FunctionType funcType;
    private FunctionProcess currProcess;
    private Color currColor;
    private FunctionLine line;

    public static Function Create(Transform trans, string prefab, Equation equation, FunctionType funcType, Color color) {
        GameObject obj = Instantiate((GameObject)Resources.Load(prefab), InGameDimentions.center, Quaternion.identity, trans) as GameObject;
        Function func = obj.GetComponent<Function>();
        func.name = equation.name;
        func.eq = equation;
        func.funcType = funcType;
        func.currColor = color;
        return func;
    }
    void Awake() {
        trans = transform;
        gameObject.SetActive(false);
    }
    void OnEnable() {
        currProcess = FunctionProcess.UNDRAWN;
        
        switch(funcType){
            case FunctionType.DISCONTINUOUS:
                line = new DiscontinuousLine(currColor);
                break;
            case FunctionType.CONTINUOUS:
                line = new ContinuousLine(currColor);
                break;
        }
    }
    void Update(){
        if(currXscale != CartesianPlane.SharedPlane.xRatio()) currXscale = CartesianPlane.SharedPlane.xRatio();
        if(currYscale != CartesianPlane.SharedPlane.yRatio()) currYscale = CartesianPlane.SharedPlane.yRatio();
        if(!currOffset.Equals(CartesianPlane.SharedPlane.getOrigin())) currOffset = CartesianPlane.SharedPlane.getOrigin();
        if(currProcess == FunctionProcess.IDLE) line.drawLines();
    }
    public void disableFunction(){
        StopAllCoroutines();
        line.destroyLines();
        gameObject.SetActive(false);
    }
    
    public IEnumerator drawFunction(int subdivisions, float drawTime, float start, float end) {
        if(currProcess != FunctionProcess.UNDRAWN){
            Debug.Log("Drawing did not RUN");
            yield break;
        }
        currProcess = FunctionProcess.DRAWING;

        line.cachePoints(eq, start, end, subdivisions);
        for (float drawTimer = 0f; drawTimer <= drawTime && drawTime != 0f; drawTimer += Time.deltaTime){
            line.drawLines(0, drawTimer/drawTime);
            yield return null;
        }
        line.drawLines(0, 1);

        currProcess = FunctionProcess.IDLE;
    }
    public IEnumerator deleteFunction(float deleteTime) {
        if(currProcess != FunctionProcess.IDLE){
            Debug.Log("Deleting did not RUN");
            yield break;
        }
        currProcess = FunctionProcess.DELETING;

         for (float deleteTimer = 0f; deleteTimer <= deleteTime && deleteTime != 0f; deleteTimer += Time.deltaTime){
            line.drawLines(deleteTimer/deleteTime, 1);
            yield return null;
        }
        line.destroyLines();

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
    AVERAGE_VAL,
    LRAM,
    RRAM,
    MRAM,
    TRAPEZOIDAL,
    SIMPSONS,
    INTEGERAL,
    MEAN_VAL,
    SLOPE_FIELD,
    WASHERS,
    SHELLS
}

public class FunctionLine{
    protected Color lineColor;
    protected float currDrawStart;  //0 to 1
    protected float currDrawEnd;  //0 to 1
    public FunctionLine(Color c){
        lineColor = c;
        initLine();
    }
    public virtual void initLine(){}
    public virtual void cachePoints(Equation eq, float startInput, float endInput, int numPoints){}
    public virtual List<Vector3> getAllPoints(){
        return new List<Vector3>();
    }
    public void drawLines(){
        drawLines(currDrawStart, currDrawEnd);
    }
    public virtual void drawLines(float start, float end){} // 0 <= start <= end <= 1
    public void setColor(Color c){
        lineColor = c;
    }
    public virtual void destroyLines(){}
}

public class DiscontinuousLine : FunctionLine{
    private List<List<Vector3>> originalPoints;
    private List<bool> isHole;
    private List<bool> isFilled;
    private List<VectorLine> lines;

    public DiscontinuousLine(Color c) : base(c){}
    public override void initLine(){
        originalPoints = new List<List<Vector3>>();
        lines = new List<VectorLine>();
        isHole = new List<bool>();
        isFilled = new List<bool>();
    }
    public override void cachePoints(Equation eq, float startInput, float endInput, int numPoints){
        float interval = (endInput - startInput) / numPoints;
        createLine();
        for (float prevInput = startInput; prevInput <= endInput; prevInput += interval){
            float currInput = prevInput + interval;
            Discontinuity d = eq.checkDiscontinuity(prevInput, currInput);
            if(d != null && d.param != currInput){
                if(d.param != prevInput) originalPoints.Last().Add(eq.getPoint(prevInput));
                for(int i = 0; i < d.holes.Count; i++) createHole(d.holes[i], d.isFilled[i]);
                createLine();
            }
            else originalPoints.Last().Add(eq.getPoint(prevInput));
        }
    }
    public override List<Vector3> getAllPoints(){
        List<Vector3> allPoints = new List<Vector3>();
        for(int i = 0; i < originalPoints.Count; i++){
            for(int j = 0; j < originalPoints[i].Count; j++) allPoints.Add(originalPoints[i][j]);
        }
        return allPoints;
    }
    private void createLine(){
        originalPoints.Add(new List<Vector3>());
        isHole.Add(false);
        isFilled.Add(false);

        VectorLine line = new VectorLine(""+originalPoints.Count, new List<Vector3>(), 10, LineType.Continuous, Joins.Fill);
        line.SetCanvas(CartesianPlane.SharedPlane.getFunctionCanvas());
        line.color = lineColor;
        line.trigger = true;
        line.collider = false;
        line.rectTransform.gameObject.tag = "Function";
        lines.Add(line);
    }
    private void createHole(Vector3 hole, bool filled){
        createLine();
        int index = originalPoints.Count - 1;
        originalPoints[index].Add(hole);
        isHole[index] = true;
        isFilled[index] = filled;
        lines[index].SetWidth(filled ? 20f : 10f);
    }
    public override void drawLines(float start, float end){
        int count = getAllPoints().Count;
        int startInput = (int)(count * start);
        int endInput = (int)(count * end);
        int currInput = 0;
        //putting in the points to be drawn
        for(int i = 0; i < originalPoints.Count; i++){
            lines[i].points3.Clear();
            if(isHole[i]){
                if(currInput >= startInput && currInput <= endInput){
                    Vector3 center = CartesianPlane.SharedPlane.pointRelativeToOrigin(originalPoints[i].First());
                    float radius = isFilled[i] ? 0.05f : 0.1f;
                    for(int j = 0; j < 30; j++) lines[i].points3.Add(originalPoints[i].First());
                    lines[i].MakeCircle(center, radius);
                }
                currInput++;
            }
            else{
                foreach(Vector3 point in originalPoints[i]){
                    if(currInput >= startInput && currInput <= endInput) lines[i].points3.Add(CartesianPlane.SharedPlane.pointRelativeToOrigin(point));
                    currInput++;
                }
            }
            lines[i].collider = lines[i].points3.Count > 1;
        }
        //cancer hole stuff
        for(int i = 0; i < isHole.Count; i++){
            if(isHole[i] && !isFilled[i]){
                for(int k = 0; k < lines.Count; k++){
                    if(!isHole[k]){
                        for(int j = 0; j < lines[k].points3.Count; j++){
                            if(Vector3.Distance(lines[k].points3[j], CartesianPlane.SharedPlane.pointRelativeToOrigin(originalPoints[i].First())) <= 0.1f){
                                lines[k].points3.RemoveAt(j--);
                            }
                        }
                    }
                }
            }
        }
        //actually drawing the lines
        foreach(VectorLine line in lines) line.Draw();
        currDrawStart = start;
        currDrawEnd = end;
    }
    public override void destroyLines(){
        VectorLine.Destroy(lines);
        initLine();
    }
    public bool hasHoles(){
        foreach(bool hole in isHole) if(hole) return true;
        return false;
    }
}
public class ContinuousLine : FunctionLine{
    private List<Vector3> originalPoints;
    private VectorLine line;

    public ContinuousLine(Color c) : base(c){}
    public override void initLine(){
        originalPoints = new List<Vector3>();

        line = new VectorLine("continuous line", new List<Vector3>(), 10, LineType.Continuous, Joins.Fill);
        line.SetCanvas(CartesianPlane.SharedPlane.getFunctionCanvas());
        line.color = lineColor;
        line.trigger = true;
        line.collider = false;
        line.rectTransform.gameObject.tag = "Function";
    }
    public override void cachePoints(Equation eq, float startInput, float endInput, int numPoints){
        float interval = (endInput - startInput) / numPoints;
        for (float currInput = startInput; currInput <= endInput; currInput += interval) originalPoints.Add(eq.getPoint(currInput));
    }
    public override List<Vector3> getAllPoints(){
        return originalPoints;
    }
    public override void drawLines(float start, float end){
        int startIndex = (int)(start * originalPoints.Count);
        int endIndex = (int)(end * originalPoints.Count);

        line.points3.Clear();
        for(int i = startIndex; i < endIndex; i++) line.points3.Add(CartesianPlane.SharedPlane.pointRelativeToOrigin(originalPoints[i]));
        if(line.points3.Count > 1) line.collider = true;
        line.Draw();

        currDrawStart = startIndex;
        currDrawEnd = endIndex;
    }
    public override void destroyLines(){
        VectorLine.Destroy(ref line);
        initLine();
    }
    public void pulseColor(){

    }
}