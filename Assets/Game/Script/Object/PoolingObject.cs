//BOMIN
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingObject : MonoBehaviour
{
    protected bool isInit = false;
    public bool IsInit { get { return isInit; } }

    [SerializeField]
    protected int initCount = 1;
    public int InitCount { get => initCount; }

    [HideInInspector]
    public int no;

    // void OnEnable(){
    //     this.OnInit();
    //     isInit = true;
    // }

    public void InitObject(){
        this.OnInit();
        isInit = true;
    }

    // void OnDisable(){
    //     Debug.Log("OnDisable");
    //     this.OnReturn();
    //     Pool.ReturnObject(this.gameObject.name,this.gameObject);
    //     isInit = false;
    // }

    public void ReturnObject(){
        
        this.OnReturn();
        Pool.ReturnObject(this.gameObject);
        isInit = false;
        
    }

    protected virtual void OnInit(){

    }
    protected virtual void OnReturn(){

    }
}
