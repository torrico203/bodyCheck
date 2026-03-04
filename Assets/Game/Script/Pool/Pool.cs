//BOMIN
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Pool : MonoBehaviour
{
    private static Pool i;
    public static Pool I
    {
        get
        {
            if (i == null)
            {
                i = FindObjectOfType<Pool>();
                if (i == null)
                {
                    GameObject singleton = new GameObject(typeof(Pool).ToString());
                    i = singleton.AddComponent<Pool>();
                    DontDestroyOnLoad(singleton);
                }
            }
            return i;
        }
    }

    // //Entitys 코드 분류에 따라, 엔티티 프리팹을 딕셔너리에 저장 
    protected Dictionary<string,ObjectPool> list = new Dictionary<string,ObjectPool>();

    protected bool loaded = false;
    public static bool Loaded { get => I.loaded; }

    void Awake()
    {
        if(i == null)
        {
            i = this;
            DontDestroyOnLoad(gameObject); // 인스턴스 유지
        }
    }

    public static void Load(){


        //초기화
        for (int i = 0; i < I.transform.childCount; i++)
        {
            //ObjectPool pool = I.transform.GetChild(i).GetComponent<ObjectPool>();
            //I.list.Add(pool.gameObject.name, pool);

            // GameObject objectPool = new GameObject(pool.gameObject.name);
            // objectPool.transform.SetParent(I.transform);
            // ObjectPool objectPoolComponent =  objectPool.AddComponent<ObjectPool>();

            // I.list.Add(pool.gameObject.name, objectPool.GetComponent<ObjectPool>());
        }

        I.StartCoroutine(I.LoadCoroutine());
    }

    IEnumerator LoadCoroutine(){
        AsyncOperationHandle<IList<GameObject>> handle =  Addressables.LoadAssetsAsync<GameObject>("Pool", obj =>
        {
            //Gets called for every loaded asset
            Debug.Log("Pool : "+obj.name);
            
            if(!I.list.ContainsKey(obj.name)){
                GameObject objectPool = new GameObject(obj.name);
                objectPool.transform.SetParent(I.transform);
                ObjectPool objectPoolComponent =  objectPool.AddComponent<ObjectPool>();

                I.list.Add(obj.name, objectPoolComponent);
            }
            //if(I.list.ContainsKey(obj.name)){
            GameObject _obj = Instantiate(obj,I.list[obj.name].transform);
            _obj.SetActive(false);
            I.list[obj.name].Obj = _obj;
            I.list[obj.name].InitCount = obj.GetComponent<PoolingObject>().InitCount;
            I.list[obj.name].Initialize();
            //}

        });
        yield return handle;
        Debug.Log("Pool Loaded");
        //Addressables.Release(handle);
        I.loaded = true;
    }

    public static GameObject GetObject(string name){
        if(I.list.ContainsKey(name)){
            return I.list[name].GetObject();
        }
        return null;
    }

    public static T GetObjectInfo<T>(string name){
        if(I.list.ContainsKey(name)){
            return I.list[name].GetObjectInfo().GetComponent<T>();
        }
        return default(T);
    }

    public static T GetObject<T>(string name){
        if(I.list.ContainsKey(name)){
            return I.list[name].GetObject().GetComponent<T>();
        }
        return default(T);
    }
    
    public static void ReturnObject(GameObject obj){
        if(I.list.ContainsKey(obj.name)){
            I.list[obj.name].ReturnObject(obj);
        }
    }
}
