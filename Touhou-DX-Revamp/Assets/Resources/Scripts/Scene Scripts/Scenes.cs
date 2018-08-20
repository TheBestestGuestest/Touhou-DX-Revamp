using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Scenes {

    private static SortedDictionary<string, object> parameters;

    public static void clearAll() {
        parameters = null;
    }
    public static void Load(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
    public static void Load(string sceneName, SortedDictionary<string, object> parameters) {
        Scenes.parameters = parameters;
        Load(sceneName);
    }
}
