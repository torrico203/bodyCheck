using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;


public class MainCamera : MonoBehaviour
{
    private static MainCamera i;
    public static MainCamera I{
        get{
            if(i == null){
                i = FindObjectOfType<MainCamera>();
                if(i == null){
                    GameObject singleton = new GameObject(typeof(MainCamera).ToString());
                    i = singleton.AddComponent<MainCamera>();
                    DontDestroyOnLoad(singleton);
                }
            }
            return i;
        }
    }

    [SerializeField] 
    private Actor target;
    public static Actor Target
    {
        get { return I.target; }
        set { I.target = value; }
    }

    private Tween curtainTween;

    [SerializeField]
    private SpriteRenderer curtain;

    [SerializeField]
    private float smoothness = 100f;

    private bool inFocus = false;
    public static bool InFocus { get; set; }

    void Awake()
    {
        i = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if(target != null) {
            Vector3 camPos = this.transform.position;
            Vector3 targetPos = target.transform.position;
            camPos.x += (targetPos.x - camPos.x)/smoothness;
            camPos.y += (targetPos.y - camPos.y)/smoothness;
            this.transform.position = camPos;
        }
        else{
            this.transform.position = new Vector3(0, 0, -10);
        }
        
    }

    public static void SetPosition(Vector3 pos){
        pos.z = -10;
        i.transform.position = pos;
    }

    public static void Focusing(Actor actor){
        
        if(I.inFocus) return;
        I.inFocus = true;
       
        Actor _target = I.target;
        I.target = actor;
        
        MainCamera.CurtainIn(1f, ()=>{
            
            I.StartCoroutine(I.Focus(_target));
        });
        
        
    }

    IEnumerator Focus(Actor actor){
        Time.timeScale = 0.3f;
        yield return new WaitForSeconds(0.3f);
        MainCamera.CurtainOut(0.3f, ()=>{
            Time.timeScale = 1f;
            target = actor;
            inFocus = false;
        });
    }

    public static void CurtainIn(float duration, Action callback = null,bool focus = true){
        if(I.target != null && focus) I.target.ActorRoot.SetSortingOrder(15);

        if(I.curtainTween != null) I.curtainTween.Kill();
        I.curtainTween = I.curtain.DOFade(1f, duration).OnComplete(()=>{
            I.curtainTween = null;
            callback?.Invoke();
        });
    }

    public static void CurtainOut(float duration, Action callback = null){

        if(I.curtainTween != null) I.curtainTween.Kill();
        I.curtainTween = I.curtain.DOFade(0f, duration).OnComplete(()=>{
            I.curtainTween = null;
            if(I.target != null) I.target.ActorRoot.SetSortingOrder(10);
            callback?.Invoke();
        });
    }
    
}
