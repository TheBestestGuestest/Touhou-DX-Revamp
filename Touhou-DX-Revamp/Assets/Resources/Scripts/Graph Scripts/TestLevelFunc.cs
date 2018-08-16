using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevelFunc : FunctionSpawner {
	void Start () {
		base.Start();

		System.Func<float, Vector3> temp = (theta) => {
            float r = 4 * Mathf.Cos(3 * theta);
            return new Vector3(r * Mathf.Cos(theta), r * Mathf.Sin(theta));
        };
        eq = new Equation(EquationType.POLAR, "r = 4cos(3theta)", temp);
		funcList.Add(Function.Create(trans, "Prefabs/Function", eq, 300, 3f, 0f, Mathf.PI));

		temp = (x) => {
            return new Vector3(x, Mathf.Tan(x));
        };
        eq = new Equation(EquationType.POLAR, "y = tan(x)", temp);
		funcList.Add(Function.Create(trans, "Prefabs/Function", eq, 300, 3f));
	}
}
