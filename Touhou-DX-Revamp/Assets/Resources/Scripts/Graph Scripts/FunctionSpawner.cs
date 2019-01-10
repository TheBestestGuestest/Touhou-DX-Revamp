using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FunctionSpawner : MonoBehaviour {
    protected Transform trans;
    protected Func<float, Vector3> temp;
    protected Equation eq;
    protected List<Function> funcList;

    public void Start() {
        trans = transform;
        funcList = new List<Function>();
    }
    public void setFunctionActiveState(int index, bool active) {
        Function func = getFunction(index);
        if(active){
            if (func.gameObject.activeInHierarchy) throw new Exception("Function is already active!");
            else func.gameObject.SetActive(true);
        }
        else{
            if (!func.gameObject.activeInHierarchy) throw new Exception("Function is already inactive!");
            else func.disableFunction();
        }
    }
    public void deactivateAll() {
        foreach (Function func in funcList) func.disableFunction();
    }
    public Function getFunction(int index){
        if (index >= funcList.Count || index < 0) throw new Exception("Function index out of range! (" + index + ")");
        return funcList[index];
    }
}
