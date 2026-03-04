using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class AddressableAsset : MonoBehaviour
{
    public void OnDestroy()
    {
        Addressables.ReleaseInstance(gameObject);
    }

}