using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equation {
    public EquationType type;
    public string name;
    private System.Func<float, Vector3> func;
    private List<Discontinuity> discontinuities;

    public Equation(EquationType type, string name, System.Func<float, Vector3> func) {
        this.type = type;
        this.name = name;
        this.func = func;
    }
    public Vector3 getPoint(float input) {
        return func(input);
    }
    public Vector3 posRelativeToPlane(float input) {
        return CartesianPlane.SharedPlane.pointRelativeToOrigin(getPoint(input));
    }

    public void addDiscontinuity(Discontinuity d){
        if(discontinuities == null) discontinuities = new List<Discontinuity>();
        discontinuities.Add(d);
    }
    public Discontinuity checkDiscontinuity(float left, float right){
        if(discontinuities == null) return null;
        foreach(Discontinuity d in discontinuities){
            if(left <= d.param && d.param <= right) return d;
        }
        return null;
    }
}

public enum EquationType { RECTANGULAR, PARAMETRIC, POLAR }

public class Discontinuity{
    public float param;
    public Vector3 leftLimit;
    public Vector3 trueValue;
    public Vector3 rightLimit;
    public List<Vector3> holes;
    public List<bool> isFilled;
    public Discontinuity(float p, Vector3 l, Vector3 t, Vector3 r){
        param = p;
        holes = new List<Vector3>();
        isFilled = new List<bool>();
        leftLimit = l;
        trueValue = t;
        rightLimit = r;

        if(!isInfinityOrNaN(l)){
            holes.Add(leftLimit);
            isFilled.Add(leftLimit.Equals(trueValue));
        }
        if(!isInfinityOrNaN(t) && !trueValue.Equals(leftLimit)){
            holes.Add(trueValue);
            isFilled.Add(true);
        }
        if(!isInfinityOrNaN(r) && !rightLimit.Equals(leftLimit) && !rightLimit.Equals(trueValue)){
            holes.Add(rightLimit);
            isFilled.Add(false);
        }
    }
    private bool isInfinityOrNaN(Vector3 point){
        return float.IsInfinity(point.x) || float.IsInfinity(point.y) || float.IsInfinity(point.z)
        || float.IsNaN(point.x) || float.IsNaN(point.y) || float.IsNaN(point.z);
    }
}