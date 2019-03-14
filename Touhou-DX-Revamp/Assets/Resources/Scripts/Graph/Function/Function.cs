using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Vectrosity;

public enum FunctionType { DISCONTINUOUS, CONTINUOUS }
public class Function : MonoBehaviour {
    public static float currXscale = 1;
    public static float currYscale = 1;
    public static Vector3 currOffset = new Vector3(0, 0);
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
        FunctionLine.lineMaterial = Instantiate(Resources.Load("Materials/Function") as Material);
        FunctionLine.lineTexture = Resources.Load("Textures/ThickLine") as Texture;
        FunctionLine.functionCanvas = GameObject.Find("Function Canvas").GetComponent<Canvas>();
    }
    void OnEnable() {
        currProcess = FunctionProcess.UNDRAWN;

        switch (funcType) {
            case FunctionType.DISCONTINUOUS:
                line = new DiscontinuousLine(currColor);
                break;
            case FunctionType.CONTINUOUS:
                line = new ContinuousLine(currColor);
                break;
        }
    }
    void Update() {
        if (currXscale != CartesianPlane.SharedPlane.xRatio()) currXscale = CartesianPlane.SharedPlane.xRatio();
        if (currYscale != CartesianPlane.SharedPlane.yRatio()) currYscale = CartesianPlane.SharedPlane.yRatio();
        if (!currOffset.Equals(CartesianPlane.SharedPlane.getOrigin())) currOffset = CartesianPlane.SharedPlane.getOrigin();
        if (currProcess == FunctionProcess.IDLE) line.drawLines();
    }
    public void disableFunction() {
        StopAllCoroutines();
        line.destroyLines();
        gameObject.SetActive(false);
    }

    public IEnumerator drawFunction(int subdivisions, float drawTime, float start, float end) {
        if (currProcess != FunctionProcess.UNDRAWN) {
            Debug.Log("Drawing did not RUN");
            yield break;
        }
        currProcess = FunctionProcess.DRAWING;

        line.cachePoints(eq, start, end, subdivisions);
        for (float drawTimer = 0f; drawTimer <= drawTime && drawTime != 0f; drawTimer += Time.deltaTime) {
            line.drawLines(0, drawTimer / drawTime);
            yield return null;
        }
        line.drawLines(0, 1);

        currProcess = FunctionProcess.IDLE;
    }
    public IEnumerator deleteFunction(float deleteTime) {
        if (currProcess != FunctionProcess.IDLE) {
            Debug.Log("Deleting did not RUN");
            yield break;
        }
        currProcess = FunctionProcess.DELETING;

        for (float deleteTimer = 0f; deleteTimer <= deleteTime && deleteTime != 0f; deleteTimer += Time.deltaTime) {
            line.drawLines(deleteTimer / deleteTime, 1);
            yield return null;
        }
        line.destroyLines();

        currProcess = FunctionProcess.UNDRAWN;
    }
    public IEnumerator test() {
        if (currProcess != FunctionProcess.IDLE) {
            Debug.Log("test did not RUN");
            yield break;
        }
        currProcess = FunctionProcess.LRAM;

        ContinuousLine cont = (ContinuousLine)line;
        yield return cont.pulseColor(Random.ColorHSV(), Random.value * 3f, 0.5f, true);

        currProcess = FunctionProcess.IDLE;
    }
    public FunctionProcess getCurrProcess() {
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