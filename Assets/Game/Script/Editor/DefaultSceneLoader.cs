#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[InitializeOnLoadAttribute]
public static class DefaultSceneLoader
{
    static DefaultSceneLoader(){
        EditorApplication.playModeStateChanged += LoadDefaultScene;
    }

    static void LoadDefaultScene(PlayModeStateChange state){
        Scene scene = SceneManager.GetActiveScene();
        // Debug.Log(scene.buildIndex);
        // Debug.Log(state);
        if(scene.name == "DevTool")
            return;
            
        if(scene.buildIndex!=0){
            if (state == PlayModeStateChange.ExitingEditMode) {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ();
            }

            if (state == PlayModeStateChange.EnteredPlayMode) {
                var go = new GameObject("Sacrificial Lamb");
                UnityEngine.Object.DontDestroyOnLoad(go);

                foreach(var root in go.scene.GetRootGameObjects())
                    UnityEngine.Object.Destroy(root);
                EditorSceneManager.LoadScene (0);
            }
        }
        
    }
}
#endif