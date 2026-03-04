//BOMIN
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;


public class Assets : MonoBehaviour
{
    
    public static Assets I { get; private set; }


    void Awake(){
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public static void CreateAsset<T>(string path, Action<T> callback,Transform parent = null){
        Addressables.InstantiateAsync(path,parent).Completed += (handle) => {
            handle.Result.AddComponent<AddressableAsset>();
            callback.Invoke(handle.Result.GetComponent<T>());
        };
    }

    public static void CreateAsset(string path, Action<GameObject> callback,Transform parent = null){
        Addressables.InstantiateAsync(path,parent).Completed += (handle) => {
            handle.Result.AddComponent<AddressableAsset>();
            callback.Invoke(handle.Result);
        };
    }

}
