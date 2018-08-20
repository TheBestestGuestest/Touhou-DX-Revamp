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
    public void activateFunction(int index) {
        if (index >= funcList.Count || index < -1) throw new Exception("Function index out of range! (" + index + ")");
        if (index == -1) index = (int)(UnityEngine.Random.value * funcList.Count);
        Function func = funcList[index];
        if (func.gameObject.activeInHierarchy) throw new Exception("Function is already active!");
        else func.gameObject.SetActive(true);
    }
    public void deactivateAll() {
        foreach(Function func in funcList) {
            func.gameObject.SetActive(false);
            func.destroyLines();
        }
    }
}
