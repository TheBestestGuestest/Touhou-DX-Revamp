using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquationList : MonoBehaviour {
    public static EquationList SharedInstance;
    private List<Function> activeFunctions;
    private Function[] displayFunctions;

    void Awake() {
        SharedInstance = this;
    }
    void Start() {
        activeFunctions = new List<Function>();
        displayFunctions = new Function[7];
    }

    void Update() {

    }

    public void showEqList() {
        for (int i = 0; i < displayFunctions.Length; i++) {
            if (displayFunctions[i] != null) {
                Function func = displayFunctions[i];
                bool isActive = func.isActive;
                string eqName = func.getEqName();
                string process = func.getCurrProcess();

                //display it somehow;
            }
        }
    }
    public void addFunction(Function func) {
        activeFunctions.Add(func);

        int indexOfLastInactive = 6;
        for (int i = 0; i < displayFunctions.Length; i++) {
            if (displayFunctions[i] == null || !displayFunctions[i].isActive) indexOfLastInactive = i;
        }
        for (int i = 1; i <= indexOfLastInactive; i++) {
            displayFunctions[i - 1] = displayFunctions[i];
        }
        //gotta "slide" out the last function
        displayFunctions[0] = func;

        showEqList();
    }

    public void removeFunction(Function func) {
        activeFunctions.Remove(func);

        showEqList();
    }
}
