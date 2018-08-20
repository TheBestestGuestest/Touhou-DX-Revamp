using UnityEngine;

public class BossFunctions : FunctionSpawner {
    void Start() {
        base.Start();

        temp = (theta) => {
            return new Vector3(theta, 0.5f * (theta + Mathf.Sin(theta * 2.0f)));
        };
        eq = new Equation(EquationType.RECTANGULAR, "y = x + sin(x)", temp);
        funcList.Add(Function.Create(trans, "Prefabs/Function", eq, 100, 1.5f, -6, 6));

        temp = (theta) => {
            return new Vector3(theta, -0.5f * (theta + Mathf.Sin(theta * 2.0f)));
        };
        eq = new Equation(EquationType.RECTANGULAR, "y = -x + sin(x)", temp);
        funcList.Add(Function.Create(trans, "Prefabs/Function", eq, 100, 1.5f, -6, 6));
    }
}