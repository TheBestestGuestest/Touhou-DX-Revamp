using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

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

    public static AsyncOperation LoadAsync(string sceneName){
        return SceneManager.LoadSceneAsync(sceneName);
    }

    public static AsyncOperation LoadAsync(string sceneName, SortedDictionary<string, object> parameters){
        Scenes.parameters = parameters;
        return LoadAsync(sceneName);
    }
}
