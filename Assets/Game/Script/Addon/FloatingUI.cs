using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class FloatingUI : MonoBehaviour
{

    private RectTransform body;

    private Vector2 _origin;

    void Awake(){
        body = GetComponent<RectTransform>();

        _origin = body.anchoredPosition;
        Go();
    }
    

    void Go(){
        //body.anchoredPosition = _origin + new Vector2(0, 100f);
        body.DOAnchorPosY(_origin.y-10f, 2f).SetEase(Ease.OutBack).OnComplete(()=>{
            body.DOAnchorPosY(_origin.y, 2f).SetEase(Ease.OutBack).OnComplete(()=>{
                Go();
            });
        });
    }



}