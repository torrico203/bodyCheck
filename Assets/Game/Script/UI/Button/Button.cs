//BOMIN
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class Button : MonoBehaviour ,IPointerDownHandler,IPointerClickHandler,IPointerExitHandler
{
    
    protected Ease ease = Ease.OutElastic;

    [SerializeField]
    protected float duration = 0.2f;

    [SerializeField]
    protected float alpha = 1f;
    [SerializeField]
    protected float disableAlpha = 0.5f;

    [SerializeField]
    protected Color color = Color.white;
    [SerializeField]
    protected Color disableColor = Color.white;

    [SerializeField]
    protected CanvasGroup canvasGroup;

    [SerializeField]
    protected Image image;
    public Image Image{
        get => image;
    }

    [SerializeField]
    protected bool animate = true;

    [SerializeField]
    protected UnityEvent action;
    public UnityEvent Action{ get => action; set => action = value; }

    protected Tween tween,disableTween;
    protected bool isDown = false;

    [SerializeField]
    protected string sound = "Button", disableSound = "ButtonDisable";
    
    [SerializeField]
    protected bool disable = false;
    public bool Disable{
        get{return disable;}
        set{
            disable = value;
            SetAlpha();
        }
    }

    [SerializeField]
    protected string disableNotice = "";

    void Awake(){
        SetAlpha();
    }

    void SetAlpha(){
        if(disable){
            if(canvasGroup != null) canvasGroup.alpha = disableAlpha;
            if(image != null) image.color = disableColor;
        }
        else{
            if(canvasGroup != null) canvasGroup.alpha = alpha;
            if(image != null) image.color = color;
        }
    }
    

    void OnGUI(){

    }


    public void OnPointerDown(PointerEventData eventData)
    {
        isDown = true;
        if(animate) transform.localScale = Vector3.one * 0.9f;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if(!isDown) return;
        if(animate){
            if(tween != null) tween.Kill();
            tween = transform.DOScale(Vector3.one, duration).SetUpdate(true).SetEase(ease);
        }
    }
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        isDown = false;
        if(animate){
            if(tween != null) tween.Kill();
            tween = transform.DOScale(Vector3.one, duration).SetUpdate(true).SetEase(ease).OnComplete(()=>{
            });
        }
        
        Execute();
    }
    
    public virtual void Execute(){
        if(disable) {
            if(disableTween != null) return;
            Vector3 pos = transform.position;
            //transform.localScale = Vector3.one;
            disableTween = transform.DOShakePosition(0.2f, 20f, 20, 90,false).SetUpdate(true).OnComplete(()=>{
                transform.position = pos;
                disableTween = null;
            });

            if(disableNotice != "")
                UI.SmallNotice(disableNotice);
            
            
            Sound.PlaySFX(disableSound);
            return;
        }
        if(sound != ""){
            Sound.PlaySFX(sound);
        }
        if(action != null) action.Invoke();
    }
}
