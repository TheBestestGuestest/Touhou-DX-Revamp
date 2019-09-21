using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFunctions {
    protected Transform trans;
    protected Func<float, Vector3> temp;
    protected Equation eq;
    protected List<Function> funcList;

    public EnemyFunctions(Transform transform) {
        trans = transform;
        funcList = new List<Function>();
        cacheAll();
    }
    protected virtual void cacheAll() { }
    protected void setFunctionActiveState(int index, bool active) {
        Function func = getFunction(index);
        if (active) {
            if (func.gameObject.activeInHierarchy) throw new Exception("Function is already active!");
            else func.gameObject.SetActive(true);
        }
        else {
            if (!func.gameObject.activeInHierarchy) throw new Exception("Function is already inactive!");
            else func.disableFunction();
        }
    }
    public void setAllActiveState(bool active) {
        for (int i = 0; i < funcList.Count; i++) setFunctionActiveState(i, active);
    }
    public Function getFunction(int index) {
        if (index >= funcList.Count || index < 0) throw new Exception("Function index out of range! (" + index + ")");
        return funcList[index];
    }
    public int getCount() {
        return funcList.Count;
    }
    public bool allFunctionsIdle() {
        foreach (Function func in funcList) if (func.getCurrProcess() != FunctionProcess.IDLE) return false;
        return true;
    }
}
