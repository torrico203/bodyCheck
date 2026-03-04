//BOMIN
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using TMPro;
using DG.Tweening;

public class Popup : MonoBehaviour
{
    [SerializeField]
    protected Transform body;
    [SerializeField]
    protected CanvasGroup canvasGroup;

    [SerializeField]
    protected UnityEvent action,earlyAction;

    private bool isClosed = true;

    
    public void OnEnable(){
        if(!UI.Loaded) return;
        Sound.PlaySFX("PopupOpen");
        earlyAction?.Invoke();
        this.transform.SetAsLastSibling();
        if(isClosed){
            isClosed = false;
            body.localScale = new Vector3(0.8f,0.8f,0.8f);
            canvasGroup.alpha = 0f;

            Sequence seq = DOTween.Sequence();
            seq.Append(body.DOScale(new Vector3(1f,1f,1f),0.3f).SetEase(Ease.OutBack).SetUpdate(true));
            seq.Join(canvasGroup.DOFade(1f,0.3f).SetUpdate(true));
            seq.OnComplete(()=>{
                action.Invoke();
            });
            seq.SetUpdate(true);
        }
        
    }

    public void Close(Action callback){
        if(isClosed) return;
        isClosed = true;
        
        Sound.PlaySFX("PopupClose");

        Sequence seq = DOTween.Sequence();
        seq.Append(body.DOScale(new Vector3(0.8f,0.8f,0.8f),0.3f).SetEase(Ease.InBack).SetUpdate(true));
        seq.Join(canvasGroup.DOFade(0f,0.3f).SetUpdate(true));
        seq.OnComplete(()=>{
            callback.Invoke();
        });
        seq.SetUpdate(true);
    }

    public void Close(){
        Close(()=>{});
    }

}
