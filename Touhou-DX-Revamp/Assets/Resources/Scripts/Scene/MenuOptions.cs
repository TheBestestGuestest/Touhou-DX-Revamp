using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuOptions : MonoBehaviour {
    private Animator anim;
    private int continueLevel;
    private bool plusCAvailable;
    private int currentSelection;
    private float downStartTime;
    private float upStartTime;
    private float holdTime = 0.32f;

    void Awake() {
        anim = GetComponent<Animator>();
    }

    void Start() {
        if (!PlayerPrefs.HasKey("+c Mode")) PlayerPrefs.SetInt("+c Mode", 0);  //sus how to indicate that +c mode exists??
        plusCAvailable = PlayerPrefs.GetInt("+c Mode") == 1;
        plusCAvailable = true;
        anim.SetBool("plusCAvailable", plusCAvailable);

        if (!PlayerPrefs.HasKey("Last Level Completed")) PlayerPrefs.SetInt("Last Level Completed", 0);
        continueLevel = PlayerPrefs.GetInt("Last Level Completed");
        continueLevel = 1;
        anim.SetBool("continueLevel", continueLevel > 0);

        if (continueLevel > 0) GameObject.Find("Continue Gray").SetActive(false);
        else GameObject.Find("Continue Black").SetActive(false);

        currentSelection = 0;
        anim.SetInteger("currSelection", currentSelection);
    }

    void Update() {
        if(getKeyInput()) anim.SetInteger("currSelection", currentSelection);
        if (Input.GetKeyDown(KeyCode.Return)) makeSelection();
    }
    private bool getKeyInput(){  //sus when up and down are held together but ok
        int originalSelection = currentSelection;
        if (Input.GetKeyDown(KeyCode.UpArrow) || 
        (!anim.IsInTransition(0) && Input.GetKey(KeyCode.UpArrow) && Time.time - upStartTime >= holdTime)) {
            if (currentSelection != -1) currentSelection = Mathf.Max(currentSelection - 1, 0);
            if (continueLevel == 0 && currentSelection == 1) currentSelection = 0;
            if (Input.GetKeyDown(KeyCode.UpArrow)) upStartTime = Time.time;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) ||
        (!anim.IsInTransition(0) && Input.GetKey(KeyCode.DownArrow) && Time.time - downStartTime >= holdTime)) {
            if (currentSelection != -1) currentSelection = Mathf.Min(currentSelection + 1, 5);
            else{
                if(continueLevel == 0) currentSelection = 2;
                else currentSelection = 1;
            }
            if (continueLevel == 0 && currentSelection == 1) currentSelection = 2;
            if (Input.GetKeyDown(KeyCode.DownArrow)) downStartTime = Time.time;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            if (currentSelection == 0 && plusCAvailable) currentSelection = -1;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            if (currentSelection == -1) currentSelection = 0;
        }
        return originalSelection != currentSelection;
    }

    private void makeSelection() {
        anim.SetTrigger("selected");
        switch (currentSelection) {
            case -1:
                //load level 1
                break;
            case 0:
                LoadingScreen.Instance.Show(Scenes.LoadAsync("Level 1"));
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
