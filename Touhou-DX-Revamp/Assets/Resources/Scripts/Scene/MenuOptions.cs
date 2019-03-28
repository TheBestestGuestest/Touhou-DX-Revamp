using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuOptions : MonoBehaviour {
    private Animator anim;
    private int continueLevel;
    private bool plusCAvailable;
    private int currentSelection;

    void Awake() {
        anim = GetComponent<Animator>();
    }

    void Start() {
        if (!PlayerPrefs.HasKey("+c Mode")) PlayerPrefs.SetInt("+c Mode", 0);  //sus how to indicate that +c mode exists??
        plusCAvailable = PlayerPrefs.GetInt("+c Mode") == 1;
        anim.SetBool("plusCAvailable", plusCAvailable);

        if (!PlayerPrefs.HasKey("Last Level Completed")) PlayerPrefs.SetInt("Last Level Completed", 0);
        continueLevel = PlayerPrefs.GetInt("Last Level Completed");
        anim.SetBool("continueLevel", continueLevel > 0);

        if (continueLevel > 0) GameObject.Find("Continue Gray").SetActive(false);
        else GameObject.Find("Continue Black").SetActive(false);

        currentSelection = 0;
        anim.SetInteger("currSelection", currentSelection);
    }

    void Update() {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f && !anim.IsInTransition(0)) {
            if (Input.GetKey(KeyCode.UpArrow)) {
                if (currentSelection != -1) currentSelection = Mathf.Max(currentSelection - 1, 0);
                if (continueLevel == 0 && currentSelection == 1) currentSelection = 0;
            }
            if (Input.GetKey(KeyCode.DownArrow)) {
                if (currentSelection != -1) currentSelection = Mathf.Min(currentSelection + 1, 5);
                if (continueLevel == 0 && currentSelection == 1) currentSelection = 2;
            }
            if (Input.GetKey(KeyCode.RightArrow)) {
                if (currentSelection == 0 && plusCAvailable) currentSelection = -1;
            }
            if (Input.GetKey(KeyCode.LeftArrow)) {
                if (currentSelection == -1) currentSelection = 0;
            }
            anim.SetInteger("currSelection", currentSelection);
        }
        if (Input.GetKeyDown(KeyCode.Return)) {
            makeSelection();
        }
    }

    private void makeSelection() {
        switch (currentSelection) {
            case -1:
                //load level 1
                break;
            case 0:
                Scenes.Load("Level 1");
                break;
            case 1:
                //load Mathf.Min(PlayerPrefs.getInt("Last Level Completed") + 1), if +c 12, else 6);
                break;
            case 2:
                //load practice scene
                break;
            case 3:
                //load manual
                break;
            case 4:
                //load settings
                break;
            case 5:
                Application.Quit();
                break;
        }
    }
}
