using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquationList : MonoBehaviour {
    public static EquationList SharedInstance;
    private List<Function> activeFunctions;

    private Text display;
    void Awake() {
        SharedInstance = this;
        display = GetComponent<Text>();
    }
    void Start() {
        activeFunctions = new List<Function>();
    }

    public void showEqList() {
        display.text = null;
        int count = 0;
        foreach (Function func in activeFunctions) {
            if (func != null && count <= 7) {
                string eqName = func.getEqName();
                string process = func.getCurrProcess();
                bool isActive = !process.Equals("Inactive");

                display.text = string.Concat(eqName, " ", process, "\n", display.text);
                count++;
            }
            else break;
        }
    }
    public void addFunction(Function func) {
        if (!activeFunctions.Contains(func)) {  //THIS IS SLOW
            if (activeFunctions.Count >= 7) {
                for (int i = 0; i < activeFunctions.Count; i++) {
                    Function unactive = activeFunctions[i];
                    if (unactive.getCurrProcess().Equals("Inactive")) {
                        activeFunctions.RemoveAt(i);
                        if (i < 7) break;
                    }
                }
            }
            activeFunctions.Add(func);
        }
        showEqList();
    }
}
