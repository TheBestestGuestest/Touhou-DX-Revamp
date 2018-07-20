using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Function : MonoBehaviour {
    private List<Equation> eqList;
    private CartesianPlane cp;

    private float drawTimer = 10f;
    private float drawRate = 10f;

    private void Awake() {
        cp = GameObject.Find("Cartesian Plane").GetComponent<CartesianPlane>();
    }
    void Start() {
        eqList = new List<Equation>();
        for (int i = 0; i < 5; i++) eqList.Add(new Equation());
        eqList[0].eq = "sin(x)";
        eqList[0].func = (x) => Mathf.Sin(x);
        eqList[0].type = EquationType.RECTANGULAR;

        eqList[1].eq = "cos(x)";
        eqList[1].func = (x) => Mathf.Cos(x);
        eqList[1].type = EquationType.RECTANGULAR;

        eqList[2].eq = "tan(x)";
        eqList[2].func = (x) => Mathf.Tan(x);
        eqList[2].type = EquationType.RECTANGULAR;

        eqList[3].eq = "y = x";
        eqList[3].func = (x) => x;
        eqList[3].type = EquationType.RECTANGULAR;

        eqList[4].eq = "r = 4cos(3theta)";
        eqList[4].func = (theta) => {
            float r = 4 * Mathf.Cos(3 * theta);
            return new Vector2(r * Mathf.Cos(theta), r * Mathf.Sin(theta));
        };
        eqList[4].type = EquationType.POLAR;
        eqList[4].start = 0;
        eqList[4].end = 4 * Mathf.PI / 3;
        eqList[4].interval = 0.01f;
    }

    void Update() {
        if (drawTimer >= drawRate) {
            drawTimer = 0f;
            StartCoroutine(drawFunction());
        }

        drawTimer += Time.deltaTime;
    }

    public IEnumerator drawFunction() {
        int functionIndex = (int)(Random.value * eqList.Count);
        Equation eq = eqList[functionIndex];

        if (eqList[functionIndex].type == EquationType.RECTANGULAR) {
            for (float i = cp.getLeftEdge(); i < cp.getRightEdge(); i += cp.getDrawInterval()) {
                Point.Create("Prefabs/Point", eq, i, drawRate / 2);
                yield return null;
            }
        }
        else {
            for (float i = eq.start; i < eq.end; i += eq.interval) {
                Point.Create("Prefabs/Point", eq, i, drawRate / 2);
                yield return null;
            }
        }
    }
}
