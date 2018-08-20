using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevelFunc : FunctionSpawner {
    void Start() {
        base.Start();

        temp = (theta) => {
            float r = 4 * Mathf.Cos(3 * theta);
            return new Vector3(r * Mathf.Cos(theta), r * Mathf.Sin(theta));
        };
        eq = new Equation(EquationType.POLAR, "r = 4cos(3theta)", temp);
        funcList.Add(Function.Create(trans, "Prefabs/Function", eq, 300, 3f, 0f, Mathf.PI));

        temp = (x) => {
            return new Vector3(Mathf.Tan(x), x);
        };
        eq = new Equation(EquationType.RECTANGULAR, "y = tan(x)", temp);
        funcList.Add(Function.Create(trans, "Prefabs/Function", eq, 300, 3f, -6, 6, 10));

        temp = (x) => {
            return new Vector3(x, Mathf.Tan(x));
        };
        eq = new Equation(EquationType.RECTANGULAR, "x = tan(y)", temp);
        funcList.Add(Function.Create(trans, "Prefabs/Function", eq, 300, 3f, -6, 6, 10));

        temp = (x) => {
            if (x <= 0) return new Vector3(x, x);
            return new Vector3(x, Mathf.Sin(x));
        };
        eq = new Equation(EquationType.RECTANGULAR, "y = x \t x <= 0 \n y = sin(x) \t x > 0", temp);
        funcList.Add(Function.Create(trans, "Prefabs/Function", eq, 300, 3f, -6, 6));
    }
}
