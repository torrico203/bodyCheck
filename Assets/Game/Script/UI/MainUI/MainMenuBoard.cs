using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MainMenuBoard : MonoBehaviour
{

    [SerializeField]
    private RectTransform rt;

    
    public void SetPosition(int i){
        RectTransform uirt = UI.I.GetComponent<RectTransform>();
        
        //Debug.Log("screen width: " + uirt.sizeDelta.x);

        rt.DOAnchorPosX((uirt.sizeDelta.x*2f) - (uirt.sizeDelta.x*i), 0.5f).SetEase(Ease.OutBack);
    }

    
}
