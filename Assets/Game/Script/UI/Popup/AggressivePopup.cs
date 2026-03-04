//BOMIN
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using DG.Tweening;

public class AggressivePopup : MonoBehaviour
{
    

    [SerializeField]
    private Transform body;

    public void OnEnable(){
        body.localScale = new Vector3(0f,1f,1f);
        body.localPosition = new Vector3(0f,400f,0f);
        body.DOScaleX(1f,0.5f).SetEase(Ease.OutBack).OnComplete(() => {
            body.localScale = new Vector3(1f,1f,1f);
            body.DOLocalMoveY(600f,2f).SetEase(Ease.OutCirc).OnComplete(() => {
                body.DOScaleX(0f,0.5f).SetEase(Ease.InBack).OnComplete(() => {
                    this.gameObject.SetActive(false);
                });
            });
        });

    }


}
