using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equation {
    public EquationType type;
    public string eq;
    public System.Func<float, dynamic> func;
    public float start;
    public float end;
    public float interval;

    public Vector3 getPoint(float input) {
        dynamic output = func(input);
        if (output is float) return new Vector3(input, output);
        else if (output is Vector2 || output is Vector3) return output;
        else throw new System.Exception("Not a valid equation!");
    }
    public Vector3 posRelativeToPlane(float input, CartesianPlane cp) {
        Vector3 output = getPoint(input);
        return cp.pointRelativeToOrigin(output);
    }
}

public enum EquationType { RECTANGULAR, PARAMETRIC, POLAR }