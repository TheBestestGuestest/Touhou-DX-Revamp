using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//function can be environmental functions, or summoned by boss functions
//2 parts: queued up and in game
//boss can spawn right on queue to summon it
public class Function : MonoBehaviour {
    private Transform trans;
    private Equation eq;
    private static Vector3 disposedPos = new Vector3(-10, 0, 0);

    private float drawTimer;
    private float drawRate = 10f;

    public float start;
    public float end;
    public float interval;
    public float drawTime;

    public bool isActive = true;  //TODO

    public static Function Create(string prefab, Equation equation) {
        Function func = (Instantiate((GameObject)Resources.Load(prefab), disposedPos, Quaternion.identity) as GameObject).GetComponent<Function>();
        func.name = prefab.Substring(prefab.LastIndexOf("/") + 1);
        func.eq = equation;
        return func;
    }

    void Awake() {
        trans = transform;
        gameObject.SetActive(false);
    }
    void OnEnable() {
        drawTimer = 10f;
        //testing purposes only
        System.Func<float, dynamic> temp = (theta) => {
            float r = 4 * Mathf.Cos(3 * theta);
            return new Vector2(r * Mathf.Cos(theta), r * Mathf.Sin(theta));
        };
        eq = new Equation(EquationType.POLAR, "r = 4cos(3theta)", temp);
        start = 0;
        end = Mathf.PI;
        interval = 0.01f;
        //testing purposes only
        
        EquationList.SharedInstance.addFunction(this);
        //TO-DO: drawing and processing
        StartCoroutine(drawFunction());
    }
    /*
    void Update() {
        
    }
    */
    public IEnumerator drawFunction() {
        //make this DEPENDENT ON TIME.DELTATIME
        if (eq.type == EquationType.RECTANGULAR) {
            for (float i = CartesianPlane.SharedPlane.getLeftEdge(); i < CartesianPlane.SharedPlane.getRightEdge(); i += CartesianPlane.SharedPlane.getDrawInterval()) {
                Point.Create("Prefabs/Point", eq, i, drawRate / 2);
                yield return null;
            }
        }
        else {
            for (float i = start; i < end; i += interval) {
                Point.Create("Prefabs/Point", eq, i, drawRate / 2);
                yield return null;
            }
        }

        gameObject.SetActive(false);
    }

    public virtual IEnumerator doProcess() {
        yield return null;
    }

    public string getEqName() {
        return eq.name;
    }
    public string getCurrProcess() {
        return "inactive";
    }
}
