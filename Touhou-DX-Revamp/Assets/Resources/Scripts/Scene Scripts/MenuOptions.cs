using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuOptions : MonoBehaviour {

    public SpriteRenderer cursor;
    private Sprite[] cursorSprites;
    public SpriteRenderer newGame;
    private Sprite[] newGameSprites;
    public SpriteRenderer plusC;
    private Sprite[] plusCSprites;
    public SpriteRenderer cont;
    private Sprite[] contSprites;
    public SpriteRenderer practice;
    private Sprite[] practiceSprites;
    public SpriteRenderer manual;
    private Sprite[] manualSprites;
    public SpriteRenderer settings;
    private Sprite[] settingsSprites;
    public SpriteRenderer exit;
    private Sprite[] exitSprites;

    private string optionsPath = "Sprites/UISprites/StartUI/";
    private int continueLevel;
    private bool plusCAvailable;
    private int currentSelection = 0;
    void Awake() {
        if (!PlayerPrefs.HasKey("+c Mode")) PlayerPrefs.SetInt("+c Mode", 0);
        if (!PlayerPrefs.HasKey("Last Level Completed")) PlayerPrefs.SetInt("Last Level Completed", 0);

        loadAllSprites();
        continueLevel = PlayerPrefs.GetInt("Last Level Completed");
        plusCAvailable = PlayerPrefs.GetInt("+c Mode") == 1;
    }

    private void loadAllSprites() {
        cursorSprites = new Sprite[2];
        cursorSprites[0] = Resources.Load<Sprite>(optionsPath + "cursor");
        cursorSprites[1] = Resources.Load<Sprite>(optionsPath + "cursor_exit");

        newGameSprites = new Sprite[2];
        newGameSprites[0] = Resources.Load<Sprite>(optionsPath + "new_game");
        newGameSprites[1] = Resources.Load<Sprite>(optionsPath + "new_game_selected");

        plusCSprites = new Sprite[2];
        plusCSprites[0] = Resources.Load<Sprite>(optionsPath + "plus_c");
        plusCSprites[1] = Resources.Load<Sprite>(optionsPath + "plus_c_selected");

        contSprites = new Sprite[3];
        contSprites[0] = Resources.Load<Sprite>(optionsPath + "continue");
        contSprites[1] = Resources.Load<Sprite>(optionsPath + "continue_selected");
        contSprites[2] = Resources.Load<Sprite>(optionsPath + "continue_unselectable");

        practiceSprites = new Sprite[2];
        practiceSprites[0] = Resources.Load<Sprite>(optionsPath + "practice");
        practiceSprites[1] = Resources.Load<Sprite>(optionsPath + "practice_selected");

        manualSprites = new Sprite[2];
        manualSprites[0] = Resources.Load<Sprite>(optionsPath + "manual");
        manualSprites[1] = Resources.Load<Sprite>(optionsPath + "manual_selected");

        settingsSprites = new Sprite[2];
        settingsSprites[0] = Resources.Load<Sprite>(optionsPath + "settings");
        settingsSprites[1] = Resources.Load<Sprite>(optionsPath + "settings_selected");

        exitSprites = new Sprite[2];
        exitSprites[0] = Resources.Load<Sprite>(optionsPath + "exit");
        exitSprites[1] = Resources.Load<Sprite>(optionsPath + "exit_selected");
    }
    void Start() {
        currentSelection = 0;
        cursor.transform.position = new Vector3(0.85f, -0.27f);
        drawAll();
    }

    private void drawAll() {
        newGame.sprite = currentSelection <= 0 ? newGameSprites[1] : newGameSprites[0];
        plusC.sprite = currentSelection == -1 ? plusCSprites[1] : (plusCAvailable ? plusCSprites[0] : null);
        cont.sprite = currentSelection == 1 ? contSprites[1] : (continueLevel > 0 ? contSprites[0] : contSprites[2]);
        practice.sprite = currentSelection == 2 ? practiceSprites[1] : practiceSprites[0];
        manual.sprite = currentSelection == 3 ? manualSprites[1] : manualSprites[0];
        settings.sprite = currentSelection == 4 ? settingsSprites[1] : settingsSprites[0];
        exit.sprite = currentSelection == 5 ? exitSprites[1] : exitSprites[0];

        cursor.sprite = currentSelection == 5 ? cursorSprites[1] : cursorSprites[0];
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            bool cursorMoved = currentSelection < 5;

            if (currentSelection <= 0) {
                if (continueLevel == 0) currentSelection = 2;
                else currentSelection = 1;
            }
            else if (currentSelection >= 5) currentSelection = 5;
            else currentSelection++;

            drawAll();
            if (cursorMoved) StartCoroutine(moveCursor());
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            bool cursorMoved = currentSelection > 0;

            if (currentSelection == 2 && !plusCAvailable) currentSelection = 0;
            else if (currentSelection <= 0) currentSelection = 0;
            else currentSelection--;

            drawAll();
            if (cursorMoved) StartCoroutine(moveCursor());
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && currentSelection == 0 && plusCAvailable) {
            currentSelection = -1;
            drawAll();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && currentSelection == -1) {
            currentSelection = 0;
            drawAll();
        }
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return)) {
            makeSelection();
        }
    }
    private IEnumerator moveCursor() {
        float totalTime = 0f;
        Vector3 destination = new Vector3(0.85f + 0.32f * Mathf.Max(0, currentSelection), -0.27f - 0.8f * Mathf.Max(0, currentSelection));
        Vector3 start = cursor.transform.position;
        while (totalTime < 0.07f) {
            cursor.transform.position = Vector3.Lerp(start, destination, totalTime / 0.07f);
            totalTime += Time.deltaTime;
            yield return null;
        }
        cursor.transform.position = destination;
    }

    private void makeSelection() {
        switch (currentSelection) {
            case -1:
                //load level 1
                break;
            case 0:
                Scenes.Load("Main");
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
