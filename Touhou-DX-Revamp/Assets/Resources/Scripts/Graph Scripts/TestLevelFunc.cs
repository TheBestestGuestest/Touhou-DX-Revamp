using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevelFunc : FunctionSpawner {
    void Start() {  //sus
        base.Start();
        /* 
        temp = (theta) => {
            float r = 4 * Mathf.Cos(3 * theta);
            return new Vector3(r * Mathf.Cos(theta), r * Mathf.Sin(theta));
        };
        eq = new Equation(EquationType.POLAR, "r = 4cos(3theta)", temp);
        funcList.Add(Function.Create(trans, "Prefabs/Function", eq));
        //subdivisions = 300, drawtime = 3f, start = 0f, end = Mathf.PI

        temp = (x) => {
            return new Vector3(x, Mathf.Tan(x));
        };
        eq = new Equation(EquationType.RECTANGULAR, "y = tan(x)", temp);
        for(int x = -10; x <= 10; x++) eq.addDiscontinuity(new Discontinuity((x + 0.5f) * Mathf.PI, float.NegativeInfinity, float.NaN, float.PositiveInfinity));
        funcList.Add(Function.Create(trans, "Prefabs/Function", eq));
        //subdivisions = 300, drawtime = 3f, start = -6f, end = 6f

        temp = (x) => {
            return new Vector3(Mathf.Tan(x), x);
        };
        eq = new Equation(EquationType.RECTANGULAR, "x = tan(y)", temp);
        for(int y = -10; y <= 10; y++) eq.addDiscontinuity(new Discontinuity((y + 0.5f) * Mathf.PI, float.NegativeInfinity, float.NaN, float.PositiveInfinity));
        funcList.Add(Function.Create(trans, "Prefabs/Function", eq));
        //subdivisions = 300, drawtime = 3f, start = -6f, end = 6f

        temp = (x) => {
            if (x <= 0) return new Vector3(x, x);
            return new Vector3(x, Mathf.Sin(x));
        };
        eq = new Equation(EquationType.RECTANGULAR, "y = x \t x <= 0 \n y = sin(x) \t x > 0", temp);
        funcList.Add(Function.Create(trans, "Prefabs/Function", eq));
        //subdivisions = 300, drawtime = 3f, start = -6f, end = 6f
        
        temp = (x) => {
            if (x < 0) return new Vector3(x, 1);
            if (x == 0) return new Vector3(0, 0.7f);
            return new Vector3(x, 1.3f);
        };
        eq = new Equation(EquationType.RECTANGULAR, "tester", temp);
        eq.addDiscontinuity(new Discontinuity(0, 1, 0.7f, 1.3f));
        funcList.Add(Function.Create(trans, "Prefabs/Function", eq));
        //subdivisions = 300, drawtime = 3f, start = -6f, end = 6f
        */
    }
}
