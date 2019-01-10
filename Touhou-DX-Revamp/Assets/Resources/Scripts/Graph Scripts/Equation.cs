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
    public float leftLimit;
    public float trueValue;
    public float rightLimit;

    public Discontinuity(float p, float l, float t, float r){
        param = p;
        leftLimit = l;
        trueValue = t;
        rightLimit = r;
    }

    public bool limitExistsAndIsNotInfinity(int approach){
        switch(approach){
            case -1:
                return !(float.IsNaN(leftLimit) || float.IsInfinity(leftLimit));
            case 0:
                return !(float.IsNaN(trueValue) || float.IsInfinity(trueValue));
            case 1:
                return !(float.IsNaN(rightLimit) || float.IsInfinity(rightLimit));
            default:
                return false;
        }
    }
}