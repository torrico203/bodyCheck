//BOMIN
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour{


    [SerializeField]
    protected GameObject obj;
    public GameObject Obj { set => obj = value; }

    [SerializeField]
    protected int initCount = 10;
    public int InitCount { get => initCount; set => initCount = value; }

    protected int currentCount = 0;


    [SerializeField]
    protected Queue<GameObject> poolingObjectQueue = new Queue<GameObject>();

    protected void Start()
    {
        //Initialize();
    }

    public void Initialize()
    {
        for(int i = 0; i < initCount; i++)
        {
            poolingObjectQueue.Enqueue(this.CreateNewObject());
        }
    }


    protected virtual GameObject CreateNewObject()
    {
        var newObj = Instantiate(obj,transform);
        newObj.name = gameObject.name;
        PoolingObject po = newObj.GetComponent<PoolingObject>();
        po.no = currentCount++;
        newObj.SetActive(false);
        
        return newObj;
    }

    public GameObject GetObject()
    {
        if(this.poolingObjectQueue.Count<=1) poolingObjectQueue.Enqueue(this.CreateNewObject());
        var obj = this.poolingObjectQueue.Dequeue();
        obj.transform.SetParent(null);
        obj.gameObject.SetActive(true);
        return obj;
    }

    public GameObject GetObjectInfo(){
        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.transform.SetParent(this.transform);
        obj.gameObject.SetActive(false);
        this.poolingObjectQueue.Enqueue(obj);
    }
}