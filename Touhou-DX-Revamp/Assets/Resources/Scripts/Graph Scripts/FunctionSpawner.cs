using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FunctionSpawner : MonoBehaviour {
	protected Transform trans;
	protected Equation eq;
	protected List<Function> funcList;

	protected void Start () {
		trans = transform;
		funcList = new List<Function>();
	}
	public void activateFunction(int index){
		if(index >= funcList.Count) throw new Exception("Function index out of range! ("+index+")");
		Function func = funcList[index];
		if(func.gameObject.activeInHierarchy) throw new Exception("Function is already active!");
		else func.gameObject.SetActive(true);
	}
}
