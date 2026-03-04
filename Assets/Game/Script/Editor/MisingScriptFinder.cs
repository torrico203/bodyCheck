#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MissingScriptFinder
{
    [MenuItem("Tools/Find Missing Scripts in Scene")]
    public static void FindMissingScriptsInScene()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>(true);
        int missingCount = 0;

        foreach (GameObject go in allObjects)
        {
            Component[] components = go.GetComponents<Component>();

            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    Debug.LogWarning($"Missing script found on GameObject: '{GetFullPath(go)}'", go);
                    missingCount++;
                }
            }
        }

        if (missingCount == 0)
        {
            Debug.Log("✅ No missing scripts found in the scene.");
        }
        else
        {
            Debug.LogWarning($"⚠️ Total missing scripts found: {missingCount}");
        }
    }

    private static string GetFullPath(GameObject obj)
    {
        string path = obj.name;
        Transform current = obj.transform;

        while (current.parent != null)
        {
            current = current.parent;
            path = current.name + "/" + path;
        }

        return path;
    }
}
#endif